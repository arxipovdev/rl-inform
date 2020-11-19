<template>
  <v-app>
    <v-app-bar app dense color="default" :clipped-left="$vuetify.breakpoint.lgAndUp">

      <v-toolbar-title to="/">
        <span class="title">App</span>
      </v-toolbar-title>

      <v-spacer />
      <v-toolbar-items>
        <v-btn text to="/login">Войти</v-btn>
        <v-btn text to="/register">Зарегистрироваться</v-btn>
      </v-toolbar-items>

    </v-app-bar>
    <nuxt />
  </v-app>
</template>

<script>
export default {
  name: 'Public'
}
</script>
