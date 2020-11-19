<template>
  <v-footer app fixed>
    <span>&copy; {{ yearNow }}</span>
  </v-footer>
</template>

<script>
export default {
  data() {
    return {
      yearNow: new Date().getFullYear()
    }
  }
}
</script>
