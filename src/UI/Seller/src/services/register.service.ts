
import { BuyerAccessRequest, RegisterModel } from '@app-seller/shared'
import { User } from '@ordercloud/headstart-sdk'
import axios, { AxiosRequestConfig } from 'axios'
import { Buyer } from 'ordercloud-javascript-sdk'

export class RegisterService {
    constructor() {
      if (typeof axios === 'undefined') {
        throw new Error(
          'Missing required peer dependency axios. This must be installed and loaded'
        )
      }
    }
    
  public getBuyers = async (): Promise<Buyer[]> => {
    return (await axios.get('https://localhost:44358/buyer')).data
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
    return (await axios.post("https://localhost:44358/register", data)).data
  }
}