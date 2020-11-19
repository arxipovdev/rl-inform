<template>
  <v-main>
    <v-container class="fill-height" fluid="fluid">
      <v-row align="center" justify="center">
        <v-col cols="12" sm="10" md="4">
          <v-card class="elevation-6">
            <v-toolbar dense color="blue" flat="flat" dark="dark">
              <v-toolbar-title>Зарегистрироваться</v-toolbar-title>
            </v-toolbar>
            <v-card-text>
              <v-form ref="form" v-model="validForm" lazy-validation="lazy-validation">
                <v-text-field v-model="user.email" :rules="emailRules" label="Email" prepend-icon="fa-envelope" />
                <v-text-field v-model="user.login" :rules="loginRules" label="Логин" prepend-icon="fa-user" />
                <v-text-field v-model="user.name" label="Имя" prepend-icon="fa-smile" />
                <v-text-field
                  v-model="user.password"
                  :rules="passwordRules"
                  label="Пароль"
                  prepend-icon="fa-lock"
                  :append-icon="showPassword ? 'fa-eye' : 'fa-eye-slash'"
                  :type="showPassword ? 'text' : 'password'"
                  counter="counter"
                  @click:append="showPassword = !showPassword"
                />
                <v-text-field
                  v-model="user.passwordConfirm"
                  :rules="passwordConfirmRules"
                  label="Еще раз пароль"
                  prepend-icon="fa-lock"
                  :append-icon="showPasswordConfirm ? 'fa-eye' : 'fa-eye-slash'"
                  :type="showPasswordConfirm ? 'text' : 'password'"
                  counter="counter"
                  @click:append="showPasswordConfirm = !showPasswordConfirm"
                />
              </v-form>
              <v-alert v-if="hasErrors" text type="error">
                <ul><li v-for="(error, i) in errors" :key="i">{{ error }}</li></ul>
              </v-alert>
            </v-card-text>
            <v-card-actions>
              <v-spacer />
              <v-btn class="btn__register" :loading="loading" :disabled="!validForm" color="blue" outlined @click="register">Ok</v-btn>
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
    validForm: false,
    user: {
      email: '',
      login: '',
      name: '',
      password: '',
      passwordConfirm: ''
    },
    showPassword: false,
    showPasswordConfirm: false,
    emailRegExp: /^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$/,
    loading: false,
    errors: []
  }),
  computed: {
    hasErrors() {
      return this.errors.length > 0
    },
    emailRules() {
      return [
        v => !!v || 'Обязательное поле',
        v => this.emailRegExp.test(v) || 'Email некорректный'
      ]
    },
    loginRules() {
      return [
        v => !!v || 'Обязательное поле',
        v => v.length <= 20 || 'Имя должно быть не более 20 символов'
      ]
    },
    passwordRules() {
      return [
        v => !!v || 'Обязательное поле',
        v => v.length > 5 || 'Пароль должен быть не менее 6 символов',
        v => new RegExp('\\d+', 'g').test(v) || 'Пароль должен иметь хотя бы одну цифру 0-9',
        v => new RegExp('[A-Z]', 'g').test(v) || 'Пароль должен иметь по крайней мере один верхний регистр (A-Z)',
        v => new RegExp('[-!$%^&*()_+|~=`{}\\[\\]:";\'<>?,.\\/@#]', 'g').test(v) || 'Пароль должен иметь по крайней мере один не буквенно-цифровой символ'
      ]
    },
    passwordConfirmRules() {
      return [
        ...this.passwordRules,
        v => (this.user.password && v === this.user.password) || 'Новые пароли не совподают'
      ]
    }
  },
  methods: {
    async register() {
      this.errors = []
      if(this.$refs.form.validate()) {
        this.loading = true
        try {
          await this.$axios.post('/account/register', this.user)
          await this.$auth.loginWith('local', {
            data: { email: this.user.email, password: this.user.password }
          })
          this.setNotify({ color: 'success', text: 'Вы успешно зарегистрировались!' })
          await this.$router.push('/')
        } catch (e) {
          this.errors = e.response.data.errors
          console.error(e)
        }
        this.loading = false
      }
    },
    setNotify(notify) {
      if(notify && notify.color && notify.text) {
        this.$store.dispatch('resetNotify', notify)
      }
    }
  }
}
</script>
