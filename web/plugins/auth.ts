// export default function({ app } : any) {
//   const { $axios, $auth } = app
//   const token : string = $auth.getToken('local')
//   if(code === 401) {
//     const { email } = store.state.auth.auth.user
//     const { refreshToken } = store.state.auth.auth.token
//
//     $axios.$post(`${process.env.baseUrl}/account/refresh`, {
//       email, refreshToken
//     }).then((res : any) => {
//       console.log(res)
//     })
//   }
// }
