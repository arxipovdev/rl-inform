import User from '~/models/user'

export default class UserCreate extends User {
  password: string
  passwordConfirm: string
  constructor(user?: UserCreate) {
    super(user)
    this.password = user ? user.password : ''
    this.passwordConfirm = user ? user.passwordConfirm : ''
  }
}
