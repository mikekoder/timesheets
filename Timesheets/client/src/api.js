import axios from 'axios';
import {SessionStorage} from 'quasar';

axios.interceptors.request.use(config => {
  var token = SessionStorage.getItem('access_token');
  config.headers = {
    Authorization: `Bearer ${token}`
  };
  if(process.env.PROD){
    config.baseURL = 'https://timesheets-test123.azurewebsites.net/api/';
  }
  else {
    config.baseURL = 'https://localhost:44332/api/';
  }
  return config;
});

export default {
  register(email,password,displayName){
    var data = {
      email: email,
      password: password,
      displayName: displayName
    };
    return axios.post('users/register', data);
  },
  login(email,password){
    var data = {
      email: email,
      password: password
    };
    return axios.post('users/login', data);
  },
  refreshToken(){
    return axios.get('users/refreshtoken');
  },
  getTimesheets(startDate,endDate){
    var query = {
      startDate: startDate.toISOString(),
      endDate: endDate.toISOString()
    };
    return axios.get('timesheets', { params: query });
  },
  getAllTimesheets(startDate,endDate){
    var query = {
      startDate: startDate.toISOString(),
      endDate: endDate.toISOString()
    };
    return axios.get('timesheets/all', { params: query });
  },
  startTimesheet(){
    return axios.post('timesheets/start');
  },
  endTimesheet(id){
    return axios.put(`timesheets/${id}/end`);
  },
  createAbsence(date, comment){
    var data = {
      date: date.toISOString(),
      comment: comment
    };
    return axios.post('timesheets/absence', data);
  },
  deleteAbsence(id){
    return axios.delete(`timesheets/absence/${id}`);
  }
}