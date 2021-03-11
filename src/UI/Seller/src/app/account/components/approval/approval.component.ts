import { Component, ChangeDetectorRef, Inject } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'
import { applicationConfiguration } from '@app-seller/config/app.config'
import { AppConfig } from '@app-seller/models/environment.types'
import { isEqual as _isEqual, set as _set, get as _get } from 'lodash'
import { User  } from 'ordercloud-javascript-sdk'
import { RegisterService } from '../services/register.service'
import { faTrash, faThumbsUp } from '@fortawesome/free-solid-svg-icons'
import { ToastrService } from 'ngx-toastr'
import { takeWhile } from 'rxjs/operators'

@Component({
  selector: 'approval',
  templateUrl: './approval.component.html',
  styleUrls: ['./approval.component.scss'],
})
export class ApprovalComponent {
  
  faTrash = faTrash
  faThumbsUp = faThumbsUp

  users: User<any>[] = []

  constructor(
    public registerService: RegisterService,
    private toastrService: ToastrService,
    @Inject(applicationConfiguration) appConfig: AppConfig
  ) {
    this.loadUsers()
  }

  async loadUsers(): Promise<void> {
    await this.registerService.getUsersWithPendingAccessApprovals().then(x => this.users = x)
  }
}
