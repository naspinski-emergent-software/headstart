import { Injectable } from '@angular/core'
import { Branding } from 'src/app/models/branding.types'
import { AppConfig } from 'src/app/models/environment.types'

@Injectable({
  providedIn: 'root',
})
export class BrandingService {
  constructor(
    private appConfig: AppConfig
  ) {}

  Get = (): Branding => {
    const urlParams = new URLSearchParams(window.location.search);
    const brand = urlParams.get('brand');
    const hasBrand: boolean = brand !== undefined && brand !== null && brand.length > 0;
    return {
      Name: hasBrand ? brand : "Default",
      BlobUrl: `${this.appConfig.middlewareUrl}/branding/${hasBrand ? brand : "_default"}.css`,
      ClientID: this.appConfig.clientID
    }
  }

  GetStylesheetUrl = (): string => {
    return 'asd'
  }

  Apply = (): void => {
    // Create link
    let link = document.createElement('link');
    link.href = this.Get().BlobUrl;
    link.rel = 'stylesheet';
    link.type = 'text/css';
    
    let head = document.getElementsByTagName('head')[0];
    let links = head.getElementsByTagName('link');
    let style = head.getElementsByTagName('style')[0];
    
    // Check if the same style sheet has been loaded already.
    let isLoaded = false;  
    for (var i = 0; i < links.length; i++) {
      var node = links[i];
      if (node.href.indexOf(link.href) > -1) {
        isLoaded = true;
      }
    }
    if (isLoaded) return;
    head.insertBefore(link, style);
  }
}
