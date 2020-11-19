import dotenv from 'dotenv'
import colors from 'vuetify/es5/util/colors'
import ru from 'vuetify/es5/locale/ru'

dotenv.config()

export default {
  ssr: false,

  head: {
    titleTemplate: '%s - app',
    title: 'app',
    meta: [
      { charset: 'utf-8' },
      { name: 'viewport', content: 'width=device-width, initial-scale=1' },
      { hid: 'description', name: 'description', content: '' }
    ],
    link: [
      { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' }
    ]
  },

  loading: { color: '#1d70ff' },

  css: [
    '@/assets/style.scss'
  ],

  plugins: [
  ],

  components: true,

  buildModules: [
    '@nuxt/typescript-build',
    '@nuxtjs/vuetify'
  ],

  modules: [
    '@nuxtjs/axios',
    '@nuxtjs/dotenv',
    '@nuxtjs/auth'
  ],

  auth: {
    localStorage: false,
    strategies: {
      local: {
        scheme: 'refresh',
        endpoints: {
          login: { url: '/account/login', method: 'post', propertyName: 'token', altProperty: 'refreshToken' },
          refresh: { url: '/account/refresh', method: 'post', propertyName: 'token', altProperty: 'refreshToken' },
          logout: { url: '/auth/logout', method: 'post' },
          user: { url: '/account/user', method: 'get' }
        },
        user: {
          property: 'user'
        },
        token: {
          property: 'token',
          type: 'Bearer',
          maxAge: 60
        },
        refreshToken: {
          property: 'refreshToken',
          maxAge: 60 * 60 * 24 * 30
        }
      }
    },
    redirect: {
      login: '/login',
      logout: '/login',
      callback: '/login',
      home: '/'
    }
  },

  axios: {
    baseURL: process.env.API_URL
  },

  vuetify: {
    defaultAssets: {
      icons: 'fa'
    },
    lang: {
      locales: { ru },
      current: 'ru'
    },
    theme: {
      light: true,
      themes: {
        light: {
          primary: colors.blue.darken2,
          accent: colors.grey.darken3,
          secondary: colors.amber.darken3,
          info: colors.teal.lighten1,
          warning: colors.amber.base,
          error: colors.deepOrange.accent4,
          success: colors.green.accent3
        }
      }
    }
  },

  build: {
  }
}
