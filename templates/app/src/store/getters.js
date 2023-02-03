const getters = {
  sidebar: state => state.app.sidebar,
  device: state => state.app.device,
  token: state => state.user.token,
  user: state => state.user.user,
  visitedViews: state => state.tagsView.visitedViews,
  cachedViews: state => state.tagsView.cachedViews,
  permissions: state => state.user.permissions,
  permissionRoutes: state => state.permission.routes
}
export default getters
