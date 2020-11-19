import { SET_DRAWER, SET_NOTIFY, SET_ENV, API_URL, BASE_URL, NODE_ENV } from '@/constants/mutationConstants'

const state = () => ({
  drawer: true,
  env: {},
  notify: {
    value: false,
    text: '',
    color: ''
  }
  // auth: { user: { roles: [] } }
})

const getters = {
  isAdmin: (state: any) => (state.auth.user.roles.includes('Admin')),
  isEditor: (state: any) => (state.auth.user.roles.includes('Editor')),
  isCustomer: (state: any) => (state.auth.user.roles.includes('Customer'))
}

const mutations = {
  [SET_DRAWER]: (state: any, payload: any) => (state.drawer = payload),
  [SET_NOTIFY]: (state: any, payload: any) => (state.notify = payload),
  [SET_ENV]: (state: any, payload: any) => (state.env = payload)
}

const actions = {
  nuxtServerInit({ commit }: any) {
    if(process.server) {
      commit('SET_ENV', {
        [NODE_ENV]: process.env.NODE_ENV || '',
        [BASE_URL]: process.env.BASE_URL || '',
        [API_URL]: process.env.API_URL || ''
      })
    }
  },
  resetNotify({ commit }: any, notify: any) {
    commit('SET_NOTIFY', { ...notify, value: true })
    setTimeout(() => commit('SET_NOTIFY', { value: false, text: '', color: '' }), 4000)
  }
}

export { state, getters, mutations, actions }
