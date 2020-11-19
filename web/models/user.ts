export default class User {
  id: string | null
  login: string
  email: string
  name: string
  roles: string[]
  constructor(user?: User) {
    this.id = user ? user.id : null
    this.login = user ? user.login : ''
    this.email = user ? user.email : ''
    this.name = user ? user.name : ''
    this.roles = user ? user.roles : []
  }
}
