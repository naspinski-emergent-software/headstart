// core services
import { NgModule } from '@angular/core'
import { RouterModule, Routes } from '@angular/router'
import { AccountComponent } from './components/account/account.component'
import { RegisterComponent } from './components/register/register.component'
import { ApprovalComponent } from './components/approval/approval.component'
import { NotificationsComponent } from './components/notifications/notifications.component'

const routes: Routes = [
  { path: '', component: AccountComponent },
  { path: 'notifications', component: NotificationsComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'approval', component: ApprovalComponent },
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AccountRoutingModule {}
