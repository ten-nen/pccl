import request from '@/utils/request'

export function login(data) {
  return request({
    url: '/auth',
    method: 'post',
    data
  })
}

export function logout() {
  return request({
    url: '/auth',
    method: 'delete'
  })
}

export function getCurrent() {
  return request({
    url: '/user',
    method: 'get'
  })
}

export function getCurrentPermissions() {
  return request({
    url: `/user/permissions`,
    method: 'get'
  })
}

export function getPagerList(query) {
  return request({
    url: '/user/pager',
    method: 'get',
    params: query
  })
}

export function addUser(data) {
  return request({
    url: '/user',
    method: 'post',
    data
  })
}

export function updateUser(id, data) {
  return request({
    url: `/user/${id}`,
    method: 'put',
    data
  })
}
