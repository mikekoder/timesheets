<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated class="glossy">
      <q-toolbar>
        <q-btn
          flat
          dense
          round
          @click="leftDrawerOpen = !leftDrawerOpen"
          aria-label="Menu"
        >
          <q-icon name="menu" />
        </q-btn>

        <q-toolbar-title>
          {{ $t('appTitle') }}
        </q-toolbar-title>
      </q-toolbar>
    </q-header>

    <q-drawer
      v-model="leftDrawerOpen"
      bordered
      content-class="bg-grey-2"
    >
      <q-list>
        <q-item :to="{name: 'index'}" exact clickable v-if="isAuthenticated">
          <q-item-section avatar>
            <q-icon name="timelapse"></q-icon>
          </q-item-section>
          <q-item-section>
            <q-item-label>{{ $t('myTimesheets') }}</q-item-label>
          </q-item-section>
        </q-item>
        <q-item :to="{name: 'overview'}" clickable v-if="isManager">
          <q-item-section avatar>
            <q-icon name="people"></q-icon>
          </q-item-section>
          <q-item-section>
            <q-item-label>{{ $t('overview') }}</q-item-label>
          </q-item-section>
        </q-item>
        <q-item :to="{name: 'register'}" clickable v-if="!isAuthenticated">
          <q-item-section avatar>
            <q-icon name="person"></q-icon>
          </q-item-section>
          <q-item-section>
            <q-item-label>{{ $t('register') }}</q-item-label>
          </q-item-section>
        </q-item>
        <q-item :to="{name: 'login'}" clickable v-if="!isAuthenticated">
          <q-item-section avatar>
            <q-icon name="lock_open"></q-icon>
          </q-item-section>
          <q-item-section>
            <q-item-label>{{ $t('login') }}</q-item-label>
          </q-item-section>
        </q-item>
        <q-item clickable v-if="isAuthenticated" @click="logout">
          <q-item-section avatar>
            <q-icon name="lock"></q-icon>
          </q-item-section>
          <q-item-section>
            <q-item-label>{{ $t('logout') }}</q-item-label>
          </q-item-section>
        </q-item>
      </q-list>
    </q-drawer>

    <q-page-container>
      <router-view @loginSuccess="loginSuccess" />
    </q-page-container>
  </q-layout>
</template>

<script>
import api from '../api'

export default {
  name: 'MyLayout',
  data () {
    return {
      leftDrawerOpen: this.$q.platform.is.desktop,
      isAuthenticated: false,
      isManager: false
    }
  },
  methods: {
    loginSuccess(accessToken, isManager){
      this.$q.sessionStorage.set('access_token', accessToken);
      this.isAuthenticated = true;
      this.isManager = isManager;
    },
    refreshToken(){
      var includedRoutes = ['index','overview'];
      if(includedRoutes.includes(this.$route.name)){
        api.refreshToken().then(response => {
          this.loginSuccess(response.data.accessToken, response.data.isManager);
        }).catch(error => {
          this.isAuthenticated = false;
          this.isManager = false;
          this.$router.replace({name: 'login'});
        });
      }
      setTimeout(this.refreshToken, 60*1000);
    },
    logout(){
      this.isAuthenticated = false;
      this.$q.sessionStorage.clear();
      this.$router.replace({name:'login'});
    }
  },
  created(){
    this.refreshToken();
  }
}
</script>

<style>
</style>
