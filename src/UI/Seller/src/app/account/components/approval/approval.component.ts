import { Component, ChangeDetectorRef, Inject } from '@angular/core'
import { applicationConfiguration } from '@app-seller/config/app.config'
import { AppConfig } from '@app-seller/models/environment.types'
import { isEqual as _isEqual, set as _set, get as _get } from 'lodash'
import { User  } from 'ordercloud-javascript-sdk'
import { RegisterService } from '../../services/register.service'
import { faThumbsDown, faThumbsUp } from '@fortawesome/free-solid-svg-icons'
import { ToastrService } from 'ngx-toastr'
import { BuyerAccessRequest, BuyerAccessRequestXp } from '../../../models/shared.types'

@Component({
  selector: 'approval',
  templateUrl: './approval.component.html',
  styleUrls: ['./approval.component.scss'],
})
export class ApprovalComponent {
  
  faThumbsDown = faThumbsDown
  faThumbsUp = faThumbsUp
  isLoaded: boolean = false

  users: User<BuyerAccessRequestXp>[] = []

  constructor(
    public registerService: RegisterService,
    private toastrService: ToastrService,
    @Inject(applicationConfiguration) appConfig: AppConfig
  ) {
    this.loadUsers()
  }

  async loadUsers(): Promise<void> {
    this.users = []
    await this.registerService.getUsersWithPendingAccessApprovals().then(x => {
      const users = x;
      users.forEach(u => u.xp.BuyerAccessRequests = u.xp.BuyerAccessRequests.filter(r => r.Approved === null))
      this.users = users
    })
    this.isLoaded = true
  }

  async processRequest(userId: string, approve: boolean, request: BuyerAccessRequest) {
    await this.registerService.processAccessRequest(userId, approve, request).then(x => {
      
      if (approve)
        this.toastrService.success(`Request for access to ${request.BuyerName} was approved`, 'Access approved');
      else
        this.toastrService.info(`Request for access to ${request.BuyerName} was denied`, 'Access denied');

      const user = this.users.find(x => x.ID === userId)
      user.xp.BuyerAccessRequests = user.xp.BuyerAccessRequests.filter(x => x.BuyerId !== request.BuyerId);
      if (user.xp.BuyerAccessRequests.length === 0)
       this.users = this.users.filter(x => x.ID !== userId)
   })
  }
}
