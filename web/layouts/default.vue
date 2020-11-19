<template>
  <v-app dark>
    <app-header />
    <app-navigation />
    <v-main>
      <v-container fluid>
        <v-overlay v-if="!isLoadedPage" :value="!isLoadedPage">
          <v-progress-circular :size="50" color="primary" indeterminate />
        </v-overlay>
        <nuxt v-else />
      </v-container>
    </v-main>
    <app-notify />
    <app-footer />
  </v-app>
</template>

<script>
import AppHeader from '@/components/header'
import AppNavigation from '@/components/navigation'
import AppFooter from '@/components/footer'
import AppNotify from '@/components/notify'

export default {
  name: 'Default',
  middleware: 'auth',
  components: {
    AppHeader,
    AppNavigation,
    AppFooter,
    AppNotify
  },
  data() {
    return {
      isLoadedPage: false
    }
  },
  mounted() {
    this.isLoadedPage = true
  }
}
</script>
