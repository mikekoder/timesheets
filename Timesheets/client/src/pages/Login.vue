<template>
  <q-page padding class="fixed-center">
    <div class="row">
      <div class="col">
        <h2>{{ $t('login') }}</h2>
      </div>
    </div>
    <div class="row q-my-md form-input">
      <div class="col">
        <q-input v-model="email" :label="$t('email')" stack-label />
      </div>
    </div>
    <div class="row q-my-md form-input">
      <div class="col">
        <q-input v-model="password" :type="showPassword ? 'text' : 'password'" :label="$t('password')" stack-label>
          <template v-slot:append>
            <q-icon :name="showPassword ? 'visibility' : 'visibility_off'" class="cursor-pointer" @click="showPassword = !showPassword" />
          </template>
        </q-input>
      </div>
    </div>
    <div class="row q-my-md">
      <div class="col">
        <q-btn @click="login" color="primary" :label="$t('login')" :disabled="!canSend"></q-btn>
      </div>
      <div class="col">
        <q-btn flat :label="$t('register')" @click="$router.replace({name: 'register'})"></q-btn>
      </div>
    </div>
  </q-page>
</template>

<style scoped>
   .form-input{
     width: 300px;
   }
</style>

<script>
import api from '../api'
export default {
  name: 'PageLogin',
  data(){
    return {
      email: '',
      password: '',
      showPassword: false
    }
  },
  computed:{
    canSend(){
      return this.email && this.password;
    }
  },
  methods:{
    async login(){
      var response = await api.login(this.email, this.password);
      if(response.status == 200){
        this.$emit('loginSuccess', response.data.accessToken, response.data.isManager);
        this.$router.replace({name: 'index'});
      }
    }
  }
}
</script>
