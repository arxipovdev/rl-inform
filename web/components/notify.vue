<template>
  <v-snackbar v-model="show" :color="notify.color" right="right" top="top" timeout="4000">
    {{ notify.text }}
    <template v-slot:action="{ attrs }">
      <v-icon small v-bind="attrs" class="snackbar__close" @click="close">fa-times</v-icon>
    </template>
  </v-snackbar>
</template>

<script>
export default {
  name: 'Notify',
  data() {
    return {
      show: false
    }
  },
  computed: {
    notify() {
      return this.$store.state.notify
    }
  },
  watch: {
    notify() {
      this.show = this.notify.value
    }
  },
  methods: {
    close() {
      this.$store.commit('SET_NOTIFY', { value: false, color: '', text: '' })
    }
  }
}
</script>
