// angular
import { Component, OnInit } from '@angular/core'
import { FormGroup, FormControl } from '@angular/forms'
import { ToastrService } from 'ngx-toastr'
import { AppConfig } from 'src/app/models/environment.types'
import { ShopperContextService } from 'src/app/services/shopper-context/shopper-context.service'
import { BrandingService } from 'src/app/services/current-user/branding.service'

@Component({
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class OCMLogin implements OnInit {
  form: FormGroup
  appName: string
  brand: string

  constructor(
    private context: ShopperContextService,
    private toasterService: ToastrService,
    public appConfig: AppConfig,
    private branding: BrandingService
  ) {}

  ngOnInit(): void {
    this.brand = this.branding.Get().Name
    this.appName = this.context.appSettings.appname
    this.form = new FormGroup({
      username: new FormControl(''),
      password: new FormControl(''),
      rememberMe: new FormControl(false),
    })
  }

  async onSubmit(): Promise<void> {
    const username = this.form.get('username').value
    const password = this.form.get('password').value
    const rememberMe = this.form.get('rememberMe').value
    try {
      await this.context.authentication.profiledLogin(
        username,
        password,
        rememberMe
      )
    } catch {
      this.toasterService.error('Invalid Login Credentials')
    }
  }

  showRegisterLink(): boolean {
    return this.context.appSettings.anonymousShoppingEnabled
  }
}
