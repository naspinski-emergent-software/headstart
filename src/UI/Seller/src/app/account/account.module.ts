import { NgModule } from '@angular/core'
import { SharedModule } from '@app-seller/shared'
import { AccountRoutingModule } from './account-routing.module'
import { AccountComponent } from './components/account/account.component'
import { RegisterComponent } from './components/register/register.component'
import { ApprovalComponent } from './components/approval/approval.component'
import { NotificationsComponent } from './components/notifications/notifications.component'
import { AccountMenuComponent } from './components/account-menu/account-menu.component'
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar'
import { AccountSummaryComponent } from './components/account-summary/account-summary.component'
import { SellerEmailNotifications } from './components/notifications/seller-email-notifications/seller-email-notifications.component'
import { RegisterService } from './services/register.service'

@NgModule({
  imports: [SharedModule, AccountRoutingModule, PerfectScrollbarModule],
  declarations: [
    AccountComponent,
    AccountSummaryComponent,
    AccountMenuComponent,
    NotificationsComponent,
    SellerEmailNotifications,
    RegisterComponent,
    ApprovalComponent
  ],
  providers: [
    RegisterService
  ]
})
export class AccountModule {}
