<template>
  <v-navigation-drawer app :value="drawer" :clipped="$vuetify.breakpoint.lgAndUp">
    <v-list dense>
      <v-list-item two-line>
        <v-list-item-avatar>
          <img src="/img/defaultUser.png" alt="user logo">
        </v-list-item-avatar>
        <v-list-item-content>
          <v-list-item-title v-if="user && user.name">{{ user.name }}</v-list-item-title>
          <v-list-item-subtitle>
            <nuxt-link to="/profile">Настройки</nuxt-link>
          </v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item>
      <v-divider />
      <v-list-item v-for="item in menuComputed" :key="item.text" :to="item.url">
        <v-list-item-action>
          <v-icon>{{ item.icon }}</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title>{{ item.text }}</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
    </v-list>
  </v-navigation-drawer>
</template>

<script>
export default {
  data() {
    return {
      menu: [
        { icon: 'fa-home', text: 'Главная', url: '/', public: true },
        { icon: 'fa-users', text: 'Пользователи', url: '/users', public: false },
        { icon: 'fa-user-tag', text: 'Роли', url: '/roles', public: false }
      ]
    }
  },
  computed: {
    drawer() {
      return this.$store.state.drawer
    },
    user() {
      if(!this.$store.state.auth.user) { return null }
      return this.$store.state.auth.user
    },
    isAdmin() {
      return this.$store.getters.isAdmin
    },
    isEditor() {
      return this.$store.getters.isEditor
    },
    menuComputed() {
      return this.isAdmin || this.isEditor
        ? this.menu
        : this.menu.filter(x => x.public)
    }
  }
}
</script>
