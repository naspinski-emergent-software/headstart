import { NgModule } from '@angular/core'
import { SharedModule } from '@app-seller/shared'
import { RegisterRoutingModule } from './register-routing.module'
import { RegisterComponent } from './components/register/register.component'
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar'
import { RegisterService } from './services/register.service'

@NgModule({
  imports: [SharedModule, RegisterRoutingModule, PerfectScrollbarModule],
  declarations: [
    RegisterComponent
  ],
  providers: [
    RegisterService
  ]
})
export class RegisterModule {}
