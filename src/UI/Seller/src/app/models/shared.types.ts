import { FormGroup } from "@angular/forms";
import { User } from "ordercloud-javascript-sdk";

export interface Route {
  title: string
  route: string
}

export interface HeaderNav {
  title: string
  routes: Route[]
}

export interface HSRoute {
  rolesWithAccess: string[]
  // this allows the routes to be narrowed based upon OC user type
  orderCloudUserTypesWithAccess?: string[]
  title: string
  route: string
  queryParams?: Record<string, any>
  // if subroutes are included, itesms will display in a dropdown
  subRoutes?: HSRoute[]
}

export interface ResourceUpdate {
  field: string
  value: any
}

export interface ResourceFormUpdate extends ResourceUpdate {
  form?: FormGroup
}

export interface SwaggerSpecProperty {
  field: string
  type: SwaggerSpecPropertyType
}

export enum SwaggerSpecPropertyType {
  String = 'string',
  Integer = 'integer',
  Object = 'object',
  Boolean = 'boolean',
  Number = 'number',
  Array = 'array',
}

export interface SummaryResourceInfoPaths {
  toPrimaryHeader: string
  toSecondaryHeader: string
  toImage: string
  toExpandable: boolean
}

export interface SummaryResourceInfoPathsDictionary {
  [resourceType: string]: SummaryResourceInfoPaths
}

export class RegisterModel {
  ID: string = ''
  FirstName: string = ''
  LastName: string= ''
  Email: string = ''
  BuyerAccessRequests: BuyerAccessRequest[] = []
  Username: string = ''
  Password: string = ''
  Active: boolean = false
}

export class BuyerAccessRequest {
  BuyerId: string = ''
  BuyerName: string = ''
  Approved: boolean = null
}
