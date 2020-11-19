<template>
  <v-layout>
    <v-flex md12>
      <client-only>
        <v-data-table :headers="headers" :items="users" :search="search" :footer-props="footerProps" :loading="loading">
          <template v-slot:top>
            <v-toolbar flat>
              <v-toolbar-title class="float-left">{{ title }}</v-toolbar-title>
              <v-spacer />
              <div class="d-flex align-center">
                <v-text-field v-model="search" label="Найти" hide-details clearable clear-icon="fa-times" class="search" />
                <v-btn small outlined color="primary" class="ml-3 mt-1" @click="createUser">
                  <v-icon small class="mr-2">fa fa-plus</v-icon>Создать
                </v-btn>
              </div>
            </v-toolbar>
          </template>
          <template v-slot:item="props">
            <tr>
              <td>{{ props.item.login }}</td>
              <td>{{ props.item.email }}</td>
              <td>{{ props.item.name }}</td>
              <td class="text-center">
                <v-tooltip top>
                  <template v-slot:activator="{ on }">
                    <v-icon class="mr-2" small dark color="primary" v-on="on" @click="getUserById(props.item.id)">fa-edit</v-icon>
                  </template>
                  <span>Редактировать</span>
                </v-tooltip>
                <v-tooltip top>
                  <template v-slot:activator="{ on }">
                    <v-icon small dark color="red" v-on="on" @click="removeUser(props.item)">fa-trash-alt</v-icon>
                  </template>
                  <span>Удалить</span>
                </v-tooltip>
              </td>
            </tr>
          </template>
        </v-data-table>
      </client-only>

      <!-- ==========================================================================================================-->
      <!-- Модальное окно-->
      <!-- ==========================================================================================================-->
      <v-dialog v-model="itemModal" persistent="persistent" max-width="500px">
        <v-card>
          <v-card-title>{{ modalTitle }}</v-card-title>
          <v-card-text>
            <v-form ref="form" v-model="validForm">
              <v-text-field v-if="isCreate" v-model="user.login" :rules="loginRules" label="Логин*" prepend-icon="fa-user" />
              <v-text-field v-model="user.email" :rules="emailRules" label="Email*" prepend-icon="fa-envelope" />
              <v-text-field v-model="user.name" label="Имя" prepend-icon="fa-smile" />
              <v-combobox v-model="user.roles" :items="rolesList" multiple clearable small-chips label="Роли" prepend-icon="fa-user-tag" />
              <v-text-field
                v-if="isCreate"
                v-model="user.password"
                :rules="passwordRules"
                label="Пароль*"
                prepend-icon="fa-lock"
                :append-icon="showPassword ? 'fa-eye' : 'fa-eye-slash'"
                :type="showPassword ? 'text' : 'password'"
                counter="counter"
                @click:append="showPassword = !showPassword"
              />
              <v-text-field
                v-if="isCreate"
                v-model="user.passwordConfirm"
                :rules="passwordConfirmRules"
                label="Пароль*"
                prepend-icon="fa-lock"
                :append-icon="showPasswordConfirm ? 'fa-eye' : 'fa-eye-slash'"
                :type="showPasswordConfirm ? 'text' : 'password'"
                counter="counter"
                @click:append="showPasswordConfirm = !showPasswordConfirm"
              />
            </v-form>
            <v-alert v-if="errors.length > 0" text type="error">
              <div class="title">Ошибки</div>
              <ul><li v-for="(error, i) in errors" :key="i">{{ error }}</li></ul>
            </v-alert>
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn color="red" text @click="itemModal = false">Закрыть</v-btn>
            <v-btn color="primary" :disabled="!validForm" :loading="isSaved" text @click="saveUser">Сохранить</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>

      <!-- ==========================================================================================================-->
      <!-- Модальное окно подтверждения удаления-->
      <!-- ==========================================================================================================-->
      <v-dialog v-model="confirmModal" persistent="persistent" max-width="400px">
        <v-card>
          <v-card-title>Удаление!</v-card-title>
          <v-card-text>Вы действительно хотите удалить пользователя "{{ user.name }}"?</v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn text="text" @click="confirmModal = false">Отменить</v-btn>
            <v-btn color="red" text @click="confirmRemove">Удалить</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>

    </v-flex>
  </v-layout>
</template>

<script>
import User from '@/models/user'
import UserCreate from '@/models/userCreate'

export default {
  name: 'Users',
  data: () => ({
    title: 'Пользователи',
    totalUsers: 0,
    users: [],
    roles: [],
    loading: true,
    pageCount: 20,
    search: '',
    footerProps: { 'items-per-page-options': [20, 50, 100, -1] },
    init: false,
    itemModal: false,
    confirmModal: false,
    user: new User(),
    validForm: false,
    isSaved: false,
    showPassword: false,
    showPasswordConfirm: false,
    headers: [
      { text: 'Логин', value: 'login' },
      { text: 'Email', value: 'email' },
      { text: 'Имя', value: 'name' },
      { text: '', value: 'actions', width: '100px' }
    ],
    errors: []
  }),
  computed: {
    rolesList() {
      return this.roles.map(x => x.name)
    },
    isCreate() {
      return this.user.id === null
    },
    modalTitle() {
      return this.isCreate ? 'Создать' : 'Редактировать'
    },
    emailRules() {
      return [
        v => !!v || 'Обязательное поле',
        v => new RegExp('^\\w+([.-]?\\w+)*@\\w+([.-]?\\w+)*(\\.\\w{2,3})+$').test(v) || 'Email некорректный'
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
        v => (this.user.password && v === this.user.password) || 'Пароли не совподают'
      ]
    }
  },
  async mounted() {
    await this.getUsers()
    await this.getRoles()
    this.init = true
  },
  methods: {
    async getUsers() {
      this.loading = true
      try {
        const { data } = await this.$axios.get('/users')
        this.users = data
      } catch (e) {
        await this.setError(e)
      }
      this.loading = false
    },
    async getRoles() {
      try {
        const { data } = await this.$axios.get('/roles')
        this.roles = data
      } catch (e) {
        await this.setError(e)
      }
    },
    async getUserById(userId) {
      try {
        const { data } = await this.$axios.get('/users/' + userId)
        this.user = new User(data)
        this.itemModal = true
      } catch (e) {
        await this.setError(e)
      }
    },
    async confirmRemove() {
      try {
        this.confirmModal = false
        await this.$axios.delete(`/users/${this.user.id}`)
        this.setNotify({ color: 'success', text: 'Пользователь успешно удален!' })
        await this.getUsers()
      } catch (e) {
        await this.setError(e)
      }
    },
    async saveUser() {
      this.errors = []
      if(this.$refs.form.validate()) {
        try {
          this.isSaved = true
          if(this.isCreate) {
            await this.$axios.post('/users/', this.user)
            this.setNotify({ color: 'success', text: 'Пользователь успешно добавлен!' })
          } else {
            await this.$axios.put('/users/' + this.user.id, this.user)
            this.setNotify({ color: 'success', text: 'Пользователь успешно изменен!' })
          }
          this.itemModal = false
          await this.getUsers()
        } catch (e) {
          await this.setError(e)
        }
        this.isSaved = false
      }
    },
    async setError(e) {
      if(e.response.status === 401) {
        await this.$router.push('/login')
      }
      this.errors = e.response.data.errors
      console.error(e)
    },
    createUser() {
      this.user = new UserCreate()
      this.itemModal = true
    },
    removeUser(user) {
      this.user = new User(user)
      this.confirmModal = true
    },
    setNotify(notify) {
      if(notify && notify.color && notify.text) {
        this.$store.dispatch('resetNotify', notify)
      }
    }
  }
}
</script>
