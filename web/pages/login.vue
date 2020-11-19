<template>
  <v-main>
    <v-container class="fill-height" fluid="fluid">
      <v-row align="center" justify="center">
        <v-overlay v-if="!isLoadedPage" :value="!isLoadedPage">
          <v-progress-circular :size="50" color="primary" indeterminate />
        </v-overlay>
        <v-col v-else cols="12" sm="6" md="4">
          <v-card class="elevation-6">
            <v-toolbar dense color="blue" dark="dark" flat="flat">
              <v-toolbar-title>Авторизация</v-toolbar-title>
            </v-toolbar>
            <v-card-text>
              <v-form ref="form" v-model="validForm" lazy-validation="lazy-validation">
                <v-text-field v-model="email" label="логин" :rules="emailRules" prepend-icon="fa-user" />
                <v-text-field
                  v-model="password"
                  label="пароль"
                  :rules="passwordRules"
                  prepend-icon="fa-lock"
                  type="password"
                  @keyup.enter="login"
                ></v-text-field>
              </v-form>
              <v-alert v-if="hasError" class="mt-3" type="error" outlined="outlined" dense="dense">Отказано в доступе, логин/пароль не верный!</v-alert>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn
                class="btn__login"
                :loading="loading"
                :disabled="!validForm"
                color="blue"
                outlined
                @click="login"
              >Войти</v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </v-main>
</template>

<script>
export default {
  layout: 'public',
  data: () => ({
    isLoadedPage: false,
    loading: false,
    validForm: false,
    email: '',
    password: '',
    hasError: false
  }),
  computed: {
    emailRules() {
      return [
        v => !!v || 'Обязательное поле'
      ]
    },
    passwordRules() {
      return [
        v => !!v || 'Обязательное поле'
      ]
    }
  },
  mounted() {
    this.isLoadedPage = true
  },
  methods: {
    async login() {
      if(this.$refs.form.validate()) {
        try {
          this.loading = true
          await this.$auth.loginWith('local', {
            data: {
              email: this.email,
              password: this.password
            }
          })
          this.loading = false
          const notify = { color: 'success', text: 'Вы успешно авторизировались!' }
          await this.$store.dispatch('resetNotify', notify)
          await this.$router.push('/')
        } catch (error) {
          this.loading = false
          this.hasError = true
          setTimeout(() => (this.hasError = false), 3000)
        }
      }
    }
  }
}
</script>
