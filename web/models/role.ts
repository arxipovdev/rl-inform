export default class Role {
  id: string | null
  name: string
  constructor(role?: Role) {
    this.id = role ? role.id : null
    this.name = role ? role.name : ''
  }
}
