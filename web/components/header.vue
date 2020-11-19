<template>
  <v-app-bar app dense dark color="blue" :clipped-left="$vuetify.breakpoint.lgAndUp">

    <v-toolbar-title to="/">
      <v-app-bar-nav-icon @click.stop="setDrawer"></v-app-bar-nav-icon>
      <span class="title">App</span>
    </v-toolbar-title>

    <v-spacer />
    <v-toolbar-items>
      <v-btn text @click="logout"><v-icon left>fa-sign-out-alt</v-icon>Выйти</v-btn>
    </v-toolbar-items>

  </v-app-bar>
</template>

<script>
export default {
  computed: {
    drawer() {
      return this.$store.state.drawer
    }
  },
  methods: {
    setDrawer() {
      this.$store.commit('SET_DRAWER', !this.drawer)
    },
    logout() {
      this.$auth.logout()
      this.$router.push('/login')
    }
  }
}
</script>
