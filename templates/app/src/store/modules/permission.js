import { asyncRoutes, constantRoutes } from '@/router'

/**
 * Use meta.role to determine if the current user has permission
 * @param permissions
 * @param route
 */
function hasPermission(permissions, route) {
  if (route.meta && route.meta.permissions) {
    return permissions.some(permission => route.meta.permissions.includes(permission))
  } else {
    return true
  }
}

/**
 * Filter asynchronous routing tables by recursion
 * @param routes asyncRoutes
 * @param permissions
 */
export function filterAsyncRoutes(routes, permissions) {
  const res = []

  routes.forEach(route => {
    const tmp = { ...route }
    if (hasPermission(permissions, tmp)) {
      if (tmp.children) {
        tmp.children = filterAsyncRoutes(tmp.children, permissions)
      }
      res.push(tmp)
    }
  })

  return res
}

export const clientPermissions = [{
  title: '用户管理',
  name: 'default.user',
  permissions: [{
    title: '用户列表',
    name: 'default.user.getpager'
  }, {
    title: '新增用户',
    name: 'default.user.create'
  }, {
    title: '编辑用户',
    name: 'default.user.update'
  }]
}, {
  title: '角色管理',
  name: 'default.role',
  permissions: [{
    title: '角色列表',
    name: 'default.role.getall'
  }, {
    title: '新增角色',
    name: 'default.role.create'
  }, {
    title: '编辑角色',
    name: 'default.role.update'
  }, {
    title: '删除角色',
    name: 'default.role.delete'
  }]
}]

const state = {
  routes: [],
  addRoutes: []
}

const mutations = {
  SET_ROUTES: (state, routes) => {
    state.addRoutes = routes
    state.routes = constantRoutes.concat(routes)
  }
}

const actions = {
  generateRoutes({ commit }, permissions) {
    return new Promise(resolve => {
      const accessedRoutes = filterAsyncRoutes(asyncRoutes, permissions)
      commit('SET_ROUTES', accessedRoutes)
      resolve(accessedRoutes)
    })
  }
}

export default {
  namespaced: true,
  state,
  mutations,
  actions
}
