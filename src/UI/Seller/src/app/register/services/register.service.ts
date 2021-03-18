import { Injectable } from '@angular/core'
import { BuyerAccessRequest, RegisterModel } from '@app-seller/shared'
import axios from 'axios'
import { Buyer, User } from 'ordercloud-javascript-sdk'
import { ocAppConfig } from '@app-seller/config/app.config'
import { AppAuthService } from '../../auth/services/app-auth.service'

@Injectable({
  providedIn: 'root',
})
export class RegisterService {

    constructor(
      private appAuthService: AppAuthService
    ) {
      if (typeof axios === 'undefined') {
        throw new Error(
          'Missing required peer dependency axios. This must be installed and loaded'
        )
      }
    }
    
  public getBuyers = async (): Promise<Buyer[]> => {
    return (await axios.get(`${ocAppConfig.middlewareUrl}/buyer`)).data
  }

  public getUsersWithPendingAccessApprovals = async (): Promise<User<any>[]> => {

    const accessToken = await this.appAuthService.fetchToken().toPromise()
    return (await axios.get(`${ocAppConfig.middlewareUrl}/adminuser/buyer-access-approval`, {
      headers: {
        Authorization: `Bearer ${accessToken}`
      }
    })).data
  }

  public postRegistration = async (registerModel: RegisterModel): Promise<User> => {
    const data = {
      username: registerModel.username,
      firstName: registerModel.firstName,
      lastName: registerModel.lastName,
      email: registerModel.email,
      password: registerModel.password,
      active: false,
      xp: {
        buyerAccessRequests: registerModel.buyerAccessRequests
      }
    }
    return (await axios.post(`${ocAppConfig.middlewareUrl}/adminuser/register`, data)).data
  }

  public processAccessRequest = async (userId: string, approved: boolean, request: BuyerAccessRequest) : Promise<any> => {
    const data = {
      UserId: userId,
      BuyerId: request.BuyerId,
      Approved: approved
    }
    const accessToken = await this.appAuthService.fetchToken().toPromise()
    return (await axios.put(`${ocAppConfig.middlewareUrl}/adminuser/buyer-access-approval`, data, {
      headers: {
        Authorization: `Bearer ${accessToken}`
      }
    })).data
  }
}