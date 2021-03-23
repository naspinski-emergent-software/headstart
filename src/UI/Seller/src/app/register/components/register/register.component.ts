import { Component, Inject } from '@angular/core'
import { FormGroup, FormControl, Validators } from '@angular/forms'
import { isEqual as _isEqual, set as _set, get as _get } from 'lodash'
import { RegisterModel } from '../../../models/shared.types'
import { Buyer  } from 'ordercloud-javascript-sdk'
import { RegisterService } from '../../services/register.service'
import {
  faTrash
} from '@fortawesome/free-solid-svg-icons'
import { ValidateStrongPassword } from '@app-seller/validators/validators'
import { applicationConfiguration } from '@app-seller/config/app.config'
import { ToastrService } from 'ngx-toastr'
import { AppConfig } from '@app-seller/models/environment.types'
import { AxiosError } from 'axios'

@Component({
  selector: 'register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
  
  faTrash = faTrash

  form: FormGroup
  register: RegisterModel = new RegisterModel()
  buyers: Buyer<any>[] = []

  get showSave(): boolean {
    return this.form.valid && this.register.BuyerAccessRequests.length > 0
  }

  constructor(
    public registerService: RegisterService,
    private toastrService: ToastrService,
    @Inject(applicationConfiguration) private appConfig: AppConfig
  ) {
    this.createRegisterForm()
    this.loadBuyers()
  }

  async loadBuyers(): Promise<void> {
    await this.registerService.getBuyers().then(x => this.buyers = x)
    if(this.buyers.length > 0) {
      this.form.controls["buyer"].setValue(this.buyers[0].ID)
    }
  }
  
  createRegisterForm(): void {
    this.form = new FormGroup({
      firstName: new FormControl('', [Validators.required]),
      lastName: new FormControl('', [Validators.required]),
      email: new FormControl('', [Validators.required]),
      username: new FormControl('', [Validators.required]),
      password: new FormControl('', [
        Validators.required,
        ValidateStrongPassword,
      ]),
      buyer: new FormControl('')
    })
  }
  
  updateRegisterFromEvent(event: any, field: string): void {
    const value = event.target.value
    const registerUpdate = { field, value }
    const updateRegisterCopy: RegisterModel = this.getRegister()
    this.register = _set(updateRegisterCopy, registerUpdate.field, registerUpdate.value)
  }

  addBuyerAccessRequest(): void {
    this.updateBuyerAccessRequest(this.form.value.buyer, false)
  }

  removeBuyerAccessRequest(buyer:string): void {
    this.updateBuyerAccessRequest(buyer, true)
  }

  updateBuyerAccessRequest(buyerId: string, isDelete: boolean): void {
    debugger
    const updateRegisterCopy: RegisterModel = this.getRegister()
    if (buyerId && buyerId.length > 0) {
      const existingBuyer = updateRegisterCopy.BuyerAccessRequests.find(x => x.BuyerId === buyerId);
      const idx = updateRegisterCopy.BuyerAccessRequests.indexOf(existingBuyer)
      
      if(isDelete && idx > -1) {
        updateRegisterCopy.BuyerAccessRequests.splice(idx, 1)
      } else if(!isDelete && idx === -1) {
        const buyer = this.buyers.find(x => x.ID === buyerId)
        updateRegisterCopy.BuyerAccessRequests.push({ BuyerId: buyer.ID, BuyerName: buyer.Name, Approved: null})
      }
    }
    this.register = updateRegisterCopy
  }

  getRegister(): RegisterModel {
    return JSON.parse(JSON.stringify(this.register))
  }
  
  async onSubmit(): Promise<void> {
    await this.registerService.postRegistration(this.register)
      .then(x => {
        if (x.Email) {
          this.toastrService.success("request saved", "Success")
          this.form.reset()
        }
      }).catch((err: AxiosError) => {
        let errorMessage = 'unknown error'
        if (err.message.indexOf(' 400 ')) {
          errorMessage = 'user exists: wrong password was supplied or user not active'
        } else if (err.message.indexOf(' 409 ')) {
          errorMessage = 'incorrect clientID specified, contact administrator'
        }
        this.toastrService.error(errorMessage, "Error submitting")
      })
  }
}
