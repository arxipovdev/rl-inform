<template>
  <v-layout>
    <v-flex md12>
      <!-- ==========================================================================================================-->
      <!-- Таблица-->
      <!-- ==========================================================================================================-->
      <client-only>
        <v-data-table :headers="headers" :items="roles" :search="search" :footer-props="footerProps" :loading="loading" loading-text="Загрузка...">
          <template v-slot:top>
            <v-toolbar flat>
              <v-toolbar-title class="float-left">{{ title }}</v-toolbar-title>
              <v-spacer />
              <div class="d-flex align-center">
                <v-text-field v-model="search" label="Найти" hide-details clearable clear-icon="fa-times" class="search" />
                <v-btn small outlined color="primary" class="ml-3 mt-1" @click="createRole">
                  <v-icon small class="mr-2">fa fa-plus</v-icon>Создать
                </v-btn>
              </div>
            </v-toolbar>
          </template>
          <template v-slot:header.name="{ header }">{{ header.text.toUpperCase() }}</template>
          <template v-slot:item="props">
            <tr>
              <td>{{ props.item.name }}</td>
              <td>
                <v-tooltip top>
                  <template v-slot:activator="{ on }">
                    <v-icon class="mr-2" small dark color="primary" v-on="on" @click="getRoleById(props.item.id)">fa-edit</v-icon>
                  </template>
                  <span>Редактировать</span>
                </v-tooltip>
                <v-tooltip top>
                  <template v-slot:activator="{ on }">
                    <v-icon small dark color="red" v-on="on" @click="removeRole(props.item)">fa-trash-alt</v-icon>
                  </template>
                  <span>Удалить</span>
                </v-tooltip>
              </td>
            </tr>
          </template>
        </v-data-table>
      </client-only>
    </v-flex>

    <!-- ============================================================================================================-->
    <!-- Модальное окно создания/редактирования-->
    <!-- ============================================================================================================-->
    <v-dialog v-model="itemModal" persistent="persistent" max-width="500px">
      <v-card>
        <v-card-title>{{ modalTitle }}</v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="validForm">
            <v-text-field v-model.trim="role.name" :rules="nameRules" label="Роль" />
          </v-form>
          <v-alert v-if="errors.length > 0" text type="error">
            <div class="title">Ошибки</div>
            <ul><li v-for="(error, i) in errors" :key="i">{{ error }}</li></ul>
          </v-alert>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn color="red" text @click="itemModal = false">Закрыть</v-btn>
          <v-btn color="blue" text :disabled="!validForm" :loading="isSaved" @click="saveRole">Сохранить</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- ============================================================================================================-->
    <!-- Модальное окно подтверждения удаления-->
    <!-- ============================================================================================================-->
    <v-dialog v-model="confirmModal" persistent="persistent" max-width="400px">
      <v-card>
        <v-card-title>Удаление!</v-card-title>
        <v-card-text>Вы действительно хотите удалить роль "{{ role.name }}"?</v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn text="text" @click="confirmModal = false">Отменить</v-btn>
          <v-btn color="red" text @click="confirmRemove">Удалить</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

  </v-layout>
</template>

<script>
import Role from '@/models/role'

export default {
  name: 'Roles',
  middleware: 'guard',
  data() {
    return {
      title: 'Роли',
      roles: [],
      errors: [],
      search: '',
      headers: [
        { text: 'Название', value: 'name' },
        { value: 'actions', text: '', sortable: false, width: '100px' }
      ],
      footerProps: { 'items-per-page-options': [10, 25, 50, -1] },
      role: new Role(),
      itemModal: false,
      confirmModal: false,
      validForm: false,
      isSaved: false
    }
  },
  computed: {
    loading() {
      return this.roles.length === 0
    },
    modalTitle() {
      return this.isCreate ? 'Создать' : 'Редактировать'
    },
    nameRules() {
      return [v => !!v || 'Обязательное поле']
    },
    isCreate() {
      return this.role.id === null
    }
  },
  async mounted() {
    await this.getRoles()
  },
  methods: {
    async getRoles() {
      try {
        const { data } = await this.$axios.get('/roles')
        this.roles = data
      } catch (e) {
        await this.setError(e)
      }
    },
    async getRoleById(roleId) {
      try {
        const { data } = await this.$axios.get(`roles/${roleId}`)
        this.role = new Role(data)
        this.itemModal = true
      } catch (e) {
        await this.setError(e)
      }
    },
    async saveRole() {
      if(this.$refs.form.validate()) {
        this.errors = []
        this.isSaved = true
        try {
          if(this.isCreate) {
            await this.$axios.post('/roles', this.role)
            this.setNotify({ color: 'success', text: 'Роль успешно добавлена!' })
          } else {
            await this.$axios.put(`roles/${this.role.id}`, this.role)
            this.setNotify({ color: 'success', text: 'Роль успешно изменена!' })
          }
          this.itemModal = false
          await this.getRoles()
        } catch (e) {
          await this.setError(e)
        }
        this.isSaved = false
      }
    },
    async confirmRemove() {
      try {
        this.confirmModal = false
        await this.$axios.delete(`roles/${this.role.id}`)
        this.setNotify({ color: 'success', text: 'Роль успешно удалена!' })
        await this.getRoles()
      } catch (e) {
        await this.setError(e)
      }
    },
    async setError(e) {
      if(e.response.status === 401) {
        await this.$router.push('/login')
      }
      this.errors = e.response.data.errors
      console.error(e)
    },
    createRole() {
      this.itemModal = true
      this.role = new Role()
    },
    removeRole(role) {
      this.role = role
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
