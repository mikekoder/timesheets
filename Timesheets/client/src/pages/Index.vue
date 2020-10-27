<template>
  <q-page padding>
    <div class="row">
      <div class="col col-xs-1 col-sm-2">
        <q-btn size="lg" color="primary" class="float-right" flat icon="chevron_left" @click="previousPeriod"></q-btn>
      </div>
      <div class="col col-xs-5 col-sm-4 col-md-2 q-pr-sm">
        <q-input v-model="startDate" mask="date">
          <template v-slot:append>
            <q-icon name="event" class="cursor-pointer">
              <q-popup-proxy>
                <q-date v-model="startDate" @input="fetchData"></q-date>
              </q-popup-proxy>
            </q-icon>
          </template>
        </q-input>
      </div>
      <div class="col col-xs-5 col-sm-4 col-md-2 q-pl-sm">
        <q-input v-model="endDate" mask="date">
          <template v-slot:append>
            <q-icon name="event" class="cursor-pointer">
              <q-popup-proxy>
                <q-date v-model="endDate" @input="fetchData"></q-date>
              </q-popup-proxy>
            </q-icon>
          </template>
        </q-input>
      </div>
      <div class="col col-xs-1 col-sm-2">
        <q-btn size="lg" color="primary" class="float-left" flat icon="chevron_right" @click="nextPeriod"></q-btn>
      </div>
    </div>
    <div class="row">
      <div class="col">
        <q-markup-table>
          <thead>
            <tr>
              <td></td>
              <td v-for="day in dates">
                {{ day }}
              </td>
              <td>
                {{ $t('total') }}
              </td>
            </tr>
          </thead>
          <tbody v-for="person in people">
            <tr v-if="isOverview" class="bg-grey-2">
              <td>
                <span class="text-subtitle2">{{ person.displayName }}</span>
              </td>
              <td :colspan="dates.length + 1"></td>
            </tr>
            <tr>
              <td>
                {{ $t('startTime') }}
              </td>
              <td v-for="entry in person.entries" :class="entry.class">
                <div v-if="entry.canStart">
                  <q-btn size="sm" color="primary" :label="$t('start')" @click="start(entry)"></q-btn>
                </div>
                <div v-else>
                  <span v-if="entry.timesheet.absence">
                    {{ entry.timesheet.comment }}
                  </span>
                  <span v-else>
                    {{ entry.timesheet.startTime }}
                  </span>
                </div>
              </td>
              <td></td>
            </tr>
            <tr>
              <td>
                {{ $t('endTime') }}
              </td>
              <td v-for="entry in person.entries" :class="entry.class">
                <div v-if="entry.canEnd">
                  <q-btn size="sm" color="primary" :label="$t('end')" @click="end(entry)"></q-btn>
                </div>
                <div v-else>
                  {{ entry.timesheet.endTime }}
                </div>
              </td>
              <td></td>
            </tr>
            <tr>
              <td>
                {{ $t('flexTime') }}
              </td>
              <td v-for="entry in person.entries" :class="entry.class">
                <div v-if="!entry.timesheet.absence">
                  {{ entry.timesheet.flexTime }}
                </div>
              </td>
              <td>
                {{ person.flexTime }}
              </td>
            </tr>
            <tr v-if="!isOverview">
              <td></td>
              <td v-for="entry in person.entries">
                <q-btn size="sm" color="secondary" :label="$t('absence')" @click="setAbsence(entry)" v-if="entry.canSetAbsence"></q-btn>
                <q-btn size="sm" color="secondary" :label="$t('deleteAbsence')" @click="deleteAbsence(entry)" v-if="entry.canDeleteAbsence"></q-btn>
              </td>
              <td>
              </td>
            </tr>
          </tbody>
        </q-markup-table>
      </div>
    </div>
  </q-page>
</template>

<style>
</style>

<script>
import moment from 'moment'
import api from '../api'
export default {
  name: 'PageIndex',
  data(){
    return {
      startDate: null,
      endDate: null,
      dates: [],
      people: []
    }
  },
  computed:{
    isOverview(){
      return this.$route.name === 'overview';
    }
  },
  watch:{
    $route(){
      this.fetchData();
    }
  },
  methods:{
    previousPeriod(){
      this.movePeriod(-1);
    },
    nextPeriod(){
      this.movePeriod(1);
    },
    movePeriod(offset){
      var start = moment(this.startDate,'YYYY/MM/DD');
      var end = moment(this.endDate,'YYYY/MM/DD').endOf('day');
      var days = end.diff(start,'days') + 1;
      if(offset < 0){
        start.subtract(days,'days');
        end.subtract(days,'days');
      }
      else if(offset > 0){
        start.add(days, 'days');
        end.add(days, 'days');
      }

      this.startDate = start.format('YYYY/MM/DD');
      this.endDate = end.format('YYYY/MM/DD');
      this.fetchData();
    },
    async start(entry){
      var response = await api.startTimesheet();
      if(response.status === 200){
        await this.fetchData();
      }
    },
    async end(entry){
      var response = await api.endTimesheet(entry.timesheet.id);
      if(response.status === 200){
        await this.fetchData();
      }
    },
    async setAbsence(entry){
      this.$q.dialog({
        title: this.$t('absence'),
        message: this.$t('absenceReason'),
        prompt: {
          model: '',
          type: 'text'
        },
        cancel: true,
        persistent: true,
        color: 'secondary'
      }).onOk(data => {
        api.createAbsence(entry.date, data).then(response => {
          this.fetchData();
        });
      });
    },
    async deleteAbsence(entry){
      var response = await api.deleteAbsence(entry.timesheet.id);
      if(response.status === 200){
        this.fetchData();
      }
    },
    async fetchData(){
      var start = moment(this.startDate,'YYYY/MM/DD');
      var end = moment(this.endDate, 'YYYY/MM/DD').endOf('day');
      var days = end.diff(start, 'days') + 1;
      var dates = [];
      for(var i = 0; i < days; i++){
        var date = moment(start).add(i, 'days');
        dates.push(date.format('DD/MM'));
      }
      this.dates = dates;

      var response;
      if(this.isOverview){
        response = await api.getAllTimesheets(start, end);
      }
      else {
        response = await api.getTimesheets(start, end);
      }
      if(response.status == 200){
        this.people = response.data.map(d => {
          var entries = [];
          for(var i = 0; i < days; i++){
            var date = moment(start).add(i, 'days');
            var isWeekend = date.isoWeekday() >= 6;
            var isFuture = date.isAfter(moment());
            var isToday = date.isSame(moment(),'date');
            var timesheet = d.timesheets.find(t => moment(t.date).isSame(date));
            var entry = {
              date: date.add(12,'hours').toDate(),
              canStart: isToday && !timesheet && !isWeekend && !this.isOverview,
              canEnd: isToday && timesheet && !timesheet.absence && !timesheet.endTime && !this.isOverview,
              canSetAbsence: (isToday || isFuture) && !timesheet && !isWeekend && !this.isOverview,
              canDeleteAbsence: (isToday || isFuture) && timesheet && timesheet.absence && !this.isOverview,
              timesheet: timesheet || {},
              class: isWeekend ? 'bg-grey-4' : (timesheet && timesheet.absence) ? 'bg-green-11' : ''
            };

            entries.push(entry);
          }

          return {
            displayName: d.displayName,
            entries: entries,
            flexTime: d.flexTime
          };
        });
      }
    }
  },
  async created(){
    this.startDate = moment().startOf('isoWeek').format('YYYY/MM/DD');
    this.endDate = moment().endOf('isoWeek').format('YYYY/MM/DD');
    await this.fetchData();
  }
}
</script>
