<template>
  <v-layout class="profile">
    <v-flex md12>
      <v-tabs v-model="tab">
        <v-tab>Основное</v-tab>
        <v-tab>Пароль</v-tab>
      </v-tabs>
      <v-tabs-items v-model="tab">
        <v-tab-item>
          <v-form ref="userForm" v-model="validUserForm" lazy-validation class="profile__form">
            <v-text-field :value="user.login" label="Логин" readonly prepend-icon="fa-user" />
            <v-text-field v-model="user.email" :rules="emailRules" label="Email*" prepend-icon="fa-envelope" />
            <v-text-field v-model="user.name" label="Имя" prepend-icon="fa-user" />
            <v-alert v-if="errorsUserForm.length > 0" text type="error">
              <ul><li v-for="(error, i) in errorsUserForm" :key="i">{{ error }}</li></ul>
            </v-alert>
            <v-btn small :disabled="!validUserForm" :loading="loadingUpdateUser" color="blue" outlined class="float-right" @click="updateUser">
              <v-icon small class="mr-1">fa-save</v-icon>Сохранить
            </v-btn>
          </v-form>
        </v-tab-item>
        <v-tab-item>
          <v-form ref="passwordForm" v-model="validPasswordForm" lazy-validation class="profile__form">
            <v-text-field
              v-model="password.passwordOld"
              :rules="requiredRules"
              label="Старый пароль*"
              prepend-icon="fa-lock"
              :append-icon="showPasswordOld ? 'fa-eye' : 'fa-eye-slash'"
              :type="showPasswordOld ? 'text' : 'password'"
              counter
              @click:append="showPasswordOld = !showPasswordOld"
            />
            <v-text-field
              v-model="password.password"
              :rules="passwordRules"
              label="Новый пароль*"
              prepend-icon="fa-lock"
              :append-icon="showPassword ? 'fa-eye' : 'fa-eye-slash'"
              :type="showPassword ? 'text' : 'password'"
              counter
              @click:append="showPassword = !showPassword"
            />
            <v-text-field
              v-model="password.passwordConfirm"
              :rules="passwordConfirmRules"
              label="Новый пароль*"
              prepend-icon="fa-lock"
              :append-icon="showPasswordConfirm ? 'fa-eye' : 'fa-eye-slash'"
              :type="showPasswordConfirm ? 'text' : 'password'"
              counter
              @click:append="showPasswordConfirm = !showPasswordConfirm"
            />
            <v-alert v-if="errorsPasswordForm.length > 0" text type="error">
              <ul><li v-for="(error, i) in errorsPasswordForm" :key="i">{{ error }}</li></ul>
            </v-alert>
            <v-btn small :disabled="!validPasswordForm" :loading="loadingChangePassword" color="blue" outlined class="mt-1 float-right" @click="changePassword">
              <v-icon small class="mr-1">fa-save</v-icon>Сохранить
            </v-btn>
          </v-form>
        </v-tab-item>
      </v-tabs-items>
    </v-flex>
  </v-layout>
</template>

<script>
import User from '@/models/user'

export default {
  name: 'Profile',
  data: () => ({
    tab: null,
    user: new User(),
    validUserForm: false,
    errorsUserForm: [],
    loadingUpdateUser: false,
    password: {
      userId: '',
      passwordOld: '',
      password: '',
      passwordConfirm: ''
    },
    validPasswordForm: false,
    loadingChangePassword: false,
    errorsPasswordForm: [],
    showPasswordOld: false,
    showPassword: false,
    showPasswordConfirm: false,
    emailRegExp: /^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$/
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
    requiredRules() {
      return [
        v => !!v || 'Обязательное поле'
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
        v => (this.password.password && v === this.password.password) || 'Новые пароли не совподают'
      ]
    }
  },
  mounted() {
    this.user = new User(this.$store.state.auth.user)
    this.password.userId = this.user.id
  },
  methods: {
    async updateUser() {
      this.errorsUserForm = []
      try {
        if(this.$refs.userForm.validate()) {
          this.loadingUpdateUser = true
          const { data } = await this.$axios.put('/users/' + this.user.id, this.user)
          this.user = new User(data)
          this.$auth.setUser(data)
          this.setNotify({ color: 'success', text: 'Данные успешно изменены!' })
        }
      } catch (e) {
        await this.setError(e)
        this.errorsUserForm = e.response.data.errors
      }
      this.loadingUpdateUser = false
    },
    async changePassword() {
      this.errorsPasswordForm = []
      if(this.$refs.passwordForm.validate()) {
        this.loadingChangePassword = true
        try {
          await this.$axios.post('/account/changePassword', this.password)
          this.setNotify({ color: 'success', text: 'Пароль успешно изменен!' })
        } catch (e) {
          await this.setError(e)
          this.errorsPasswordForm = e.response.data.errors
        }
        this.loadingChangePassword = false
      }
    },
    async setError(e) {
      if(e.response.status === 401) {
        await this.$router.push('/login')
      }
      console.error(e)
    },
    setNotify(notify) {
      if(notify && notify.color && notify.text) {
        this.$store.dispatch('resetNotify', notify)
      }
    }
  }
}
</script>
