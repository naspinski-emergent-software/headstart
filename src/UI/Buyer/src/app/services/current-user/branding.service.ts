import { Injectable } from '@angular/core'
import { Branding } from 'src/app/models/branding.types'

@Injectable({
  providedIn: 'root',
})
export class BrandingService {
  constructor() {}

  Get(): Branding {
    const urlParams = new URLSearchParams(window.location.search);
    const brand = urlParams.get('brand');
    return brand === undefined || brand == null || brand.length < 1
      ? { Name: "Default", BlobUrl: 'https://stgordercloud.blob.core.windows.net/branding-css/_default.css' }
      : { Name: brand, BlobUrl: `https://stgordercloud.blob.core.windows.net/branding-css/${brand.toLowerCase()}.css` }
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
