export default function({ store, redirect }: any) {
  const isAdmin = store.getters.isAdmin
  const isEditor = store.getters.isEditor
  if(!(isAdmin || isEditor)) { return redirect('/') }
}
