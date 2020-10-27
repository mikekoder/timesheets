using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timesheets.Data;
using Timesheets.Models;

namespace Timesheets.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class TimesheetsController : BaseController
    {
        private readonly ITimesheetRepository _timesheetRepository;
        private readonly IMapper _mapper;
        public TimesheetsController(ITimesheetRepository timesheetRepository, IMapper mapper)
        {
            _timesheetRepository = timesheetRepository;
            _mapper = mapper;
        }

        [HttpPost("start")]
        public IActionResult Start()
        {
            var userId = GetUserId();
            var startTime = DateTime.Now;
            var existingTimesheet = _timesheetRepository.GetTimesheet(userId, startTime.Date);
            if(existingTimesheet != null)
            {
                return BadRequest();
            }

            var timesheet = new Timesheet
            {
                UserId = userId,
                Date = startTime.Date,
                StartTime = startTime.TimeOfDay,
            };

            _timesheetRepository.CreateTimesheet(timesheet);
            var result = _mapper.Map<TimesheetResponse>(timesheet);
            CalculateFlexTime(result);
            return Ok(result);
        }
        [HttpPut("{id}/end")]
        public IActionResult End(int id)
        {
            var timesheet = _timesheetRepository.GetTimesheet(id);
            if(timesheet == null || timesheet.UserId != GetUserId() || timesheet.Absence)
            {
                return BadRequest();
            }

            timesheet.EndTime = DateTime.Now.TimeOfDay;
            _timesheetRepository.UpdateTimesheet(timesheet);
            var result = _mapper.Map<TimesheetResponse>(timesheet);
            CalculateFlexTime(result);
            return Ok(result);
        }
        [HttpGet("")]
        public IActionResult List(DateTime startDate, DateTime endDate)
        {
            var timesheets = _timesheetRepository.GetTimesheets(GetUserId(), startDate.Date, endDate.Date);
            var result = new[]
            {
                new UserTimesheetListResponse
                {
                    Timesheets = _mapper.Map<TimesheetResponse[]>(timesheets)
                }
            };

            CalculateFlexTime(result);
            return Ok(result);
        }
        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public IActionResult ListAll(DateTime startDate, DateTime endDate)
        {
            var timesheets = _timesheetRepository.GetTimesheets(startDate.Date, endDate.Date);
            var result = timesheets.GroupBy(t => (t.UserId, t.DisplayName))
                .OrderBy(g => g.Key.DisplayName)
                .Select(g => new UserTimesheetListResponse
                {
                    DisplayName = g.Key.DisplayName,
                    Timesheets = _mapper.Map<TimesheetResponse[]>(g)
                }).ToArray();

            foreach(var group in result)
            {
                CalculateFlexTime(group);
            }
           
            return Ok(result);
        }

        [HttpPost("absence")]
        public IActionResult CreateAbsence([FromBody]AbsenceRequest request)
        {
            var userId = GetUserId();

            var existing = _timesheetRepository.GetTimesheet(userId, request.Date.Date);
            if(existing != null)
            {
                return BadRequest();
            }

            var absence = new Timesheet
            {
                UserId = userId,
                Date = request.Date.Date,
                Comment = request.Comment,
                Absence = true
            };
            _timesheetRepository.CreateTimesheet(absence);
            var result = _mapper.Map<TimesheetResponse>(absence);
            return Ok(result);
        }
        [HttpDelete("absence/{id}")]
        public IActionResult DeleteAbsence(int id)
        {
            var timesheet = _timesheetRepository.GetTimesheet(id);
            if(timesheet == null || timesheet.UserId != GetUserId() || !timesheet.Absence)
            {
                return BadRequest();
            }

            _timesheetRepository.DeleteTimesheet(id);
            return Ok();
        }

        private void CalculateFlexTime(params UserTimesheetListResponse[] timesheetLists)
        {
            foreach (var list in timesheetLists)
            {
                CalculateFlexTime(list.Timesheets);

                var timesheets = list.Timesheets.Where(t => t.FlexTime.HasValue);
                list.FlexTime = TimeSpan.FromSeconds(timesheets.Sum(t => t.FlexTime.Value.TotalSeconds));
            }
        }
        private void CalculateFlexTime(params TimesheetResponse[] timesheets)
        {
            foreach(var timesheet in timesheets.Where(t => !t.Absence && t.EndTime.HasValue))
            {
                var workingHours = timesheet.EndTime.Value - timesheet.StartTime;
                timesheet.FlexTime = workingHours - TimeSpan.FromHours(7.5);
                if(workingHours.TotalHours > 6)
                {
                    timesheet.FlexTime = timesheet.FlexTime.Value - TimeSpan.FromMinutes(30);
                }
            }
        }
    }
}
