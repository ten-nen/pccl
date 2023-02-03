<template>
  <div class="app-container">

    <div class="filter-container">
      <el-button v-if="hasPermission('default.role.create')" class="filter-item" style="margin-left: 10px;" type="primary" icon="el-icon-edit" @click="handleAdd">
        新增角色
      </el-button>
    </div>

    <el-table :data="rolesList" style="width: 100%;margin-top:30px;" border>
      <el-table-column align="center" label="角色名称">
        <template slot-scope="scope">
          {{ scope.row.name }}
        </template>
      </el-table-column>
      <el-table-column align="center" label="操作" width="300">
        <template slot-scope="scope">
          <el-button v-if="scope.row.name!='admin'&&hasPermission('default.role.update')" type="primary" size="small" @click="handleEdit(scope)">编辑</el-button>
          <el-button v-if="scope.row.name!='admin'&&hasPermission('default.role.delete')" type="danger" size="small" @click="handleDelete(scope)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog :visible.sync="dialogVisible" :title="dialogType==='edit'?'编辑角色':'新增角色'">
      <el-form ref="form" :rules="rules" :model="role" label-width="80px" label-position="rigth">
        <el-form-item label="角色名称" prop="name">
          <el-input v-model="role.name" placeholder="请输入角色名称" />
        </el-form-item>
        <el-form-item label="权限列表">
          <el-tree
            ref="tree"
            :check-strictly="checkStrictly"
            :data="permissionsData"
            :props="defaultProps"
            show-checkbox
            node-key="name"
            class="permission-tree"
          />
        </el-form-item>
      </el-form>
      <div style="text-align:right;">
        <el-button type="danger" @click="dialogVisible=false">取消</el-button>
        <el-button type="primary" :loading="loading" @click="confirmSave">保存</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { deepClone, hasPermission } from '@/utils'
import { clientPermissions } from '@/store/modules/permission'
import { getRoles, addRole, deleteRole, updateRole } from '@/api/role'

const defaultRole = {
  id: '',
  name: '',
  rolePermissions: []
}

export default {
  data() {
    return {
      loading: false,
      role: Object.assign({}, defaultRole),
      permissions: [],
      rolesList: [],
      dialogVisible: false,
      dialogType: 'new',
      checkStrictly: false,
      permissionDialogVisible: false,
      defaultProps: {
        children: 'permissions',
        label: 'title'
      },
      rules: {
        name: [
          { required: true, message: '不能为空', trigger: 'blur' },
          { max: 16, message: '长度不能大于16个字符', trigger: 'blur' }
        ]
      }
    }
  },
  computed: {
    permissionsData() {
      return clientPermissions
    }
  },
  created() {
    this.getRoles()
  },
  methods: {
    hasPermission,
    async getRoles() {
      const res = await getRoles()
      this.rolesList = res
    },
    handleAdd() {
      this.role = Object.assign({}, defaultRole)
      this.dialogType = 'new'
      this.dialogVisible = true
      this.$nextTick(() => {
        this.$refs['form'].clearValidate()
        this.$refs.tree.setCheckedNodes([])
      })
    },
    handleEdit(scope) {
      this.dialogType = 'edit'
      this.dialogVisible = true
      this.checkStrictly = true
      this.role = deepClone(scope.row)
      this.$nextTick(() => {
        this.$refs['form'].clearValidate()
        this.$refs.tree.setCheckedNodes(this.role.rolePermissions)
        // set checked state of a node not affects its father and child nodes
        this.checkStrictly = false
      })
    },
    handleDelete({ $index, row }) {
      this.$confirm('数据删除后无法恢复，确定删除吗?', '提示信息', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      })
        .then(async() => {
          await deleteRole(row.id)
          this.rolesList.splice($index, 1)
          this.$message({
            type: 'success',
            message: '删除成功!'
          })
        })
        .catch(err => { console.error(err) })
    },
    confirmSave() {
      const isEdit = this.dialogType === 'edit'

      this.$refs['form'].validate((valid) => {
        if (valid) {
          this.loading = true
          this.role.RolePermissionNames = this.$refs.tree.getCheckedNodes(false, true).map(x => x.name)
          delete this.role.rolePermissions
          if (isEdit) {
            updateRole(this.role.id, this.role).then(() => {
              console.log(this.role)
              for (let index = 0; index < this.rolesList.length; index++) {
                if (this.rolesList[index].id === this.role.id) {
                  this.rolesList.splice(index, 1, Object.assign({}, this.role))
                  break
                }
              }
              this.loading = false
              this.dialogVisible = false
              this.$notify({
                title: '角色保存成功',
                dangerouslyUseHTMLString: true,
                message: `
            <div>角色名称: ${this.role.name}</div>
          `,
                type: 'success'
              })
            }).catch(() => {
              this.loading = false
            })
          } else {
            addRole(this.role).then(data => {
              this.role.id = data.id
              this.rolesList.push(this.role)
              this.loading = false
              this.dialogVisible = false
              this.$notify({
                title: '角色保存成功',
                dangerouslyUseHTMLString: true,
                message: `
            <div>角色名称: ${this.role.name}</div>
          `,
                type: 'success'
              })
            }).catch(() => {
              this.loading = false
            })
          }
        } else {
          console.log('error submit!!')
          return false
        }
      })
    }
  }
}
</script>

<style lang="scss" scoped>
.app-container {
  .roles-table {
    margin-top: 30px;
  }
  .permission-tree {
    margin-bottom: 30px;
  }
}
</style>
