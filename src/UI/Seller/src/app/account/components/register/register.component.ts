import { Component, ChangeDetectorRef, Inject } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { applicationConfiguration } from '@app-seller/config/app.config'
import { FormGroup, FormControl, Validators } from '@angular/forms'
import { AppConfig } from '@app-seller/models/environment.types'
import { isEqual as _isEqual, set as _set, get as _get } from 'lodash'
import { RegisterModel, BuyerAccessRequest } from '../../../models/shared.types'
import { AdminUsers, User, Buyer  } from 'ordercloud-javascript-sdk'
import { RegisterService } from '../../../../services/register.service'
import {
  faTrash
} from '@fortawesome/free-solid-svg-icons'
import { ValidateStrongPassword } from '@app-seller/validators/validators'

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
  registerService: RegisterService = new RegisterService()

  constructor(
    router: Router,
    activatedRoute: ActivatedRoute,
    changeDetectorRef: ChangeDetectorRef,
    @Inject(applicationConfiguration) appConfig: AppConfig
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
    const updateRegisterCopy: RegisterModel = this.getRegister()
    if (buyerId && buyerId.length > 0) {
      const existingBuyer = updateRegisterCopy.buyerAccessRequests.find(x => x.buyerId === buyerId);
      const idx = updateRegisterCopy.buyerAccessRequests.indexOf(existingBuyer)
      
      if(isDelete && idx > -1) {
        updateRegisterCopy.buyerAccessRequests.splice(idx, 1)
      } else if(!isDelete && idx === -1) {
        const buyer = this.buyers.find(x => x.ID === buyerId)
        updateRegisterCopy.buyerAccessRequests.push({ buyerId: buyer.ID, buyerName: buyer.Name})
      }
    }
    this.register = updateRegisterCopy
  }

  getRegister(): RegisterModel {
    return JSON.parse(JSON.stringify(this.register))
  }

  async save(): Promise<User> {
    const user: User = {
      Username: this.form.value.FirstName,
      FirstName: this.form.value.FirstName,
      LastName: this.form.value.LastName,
      Email: this.form.value.FirstName,
      Active: false,
      xp: {
        BuyerRequests: this.register.buyerAccessRequests.map(x => ({ buyer: x, approved: null }))
      }
    }

    return await AdminUsers.Save(this.form.value.Username, user)
  }
  
  async onSubmit(): Promise<void> {
    await this.registerService.postRegistration(this.register)
      .then(x => {
        if (x.Email) {
          this.form.reset()
        }
      })
  }

  showSave(): boolean {
    return this.form.valid && this.register.buyerAccessRequests.length > 0
  }
}
