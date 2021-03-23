import { Component, OnInit } from '@angular/core'
import { ShopperContextService } from './services/shopper-context/shopper-context.service'
import { StaticPageService } from './services/static-page/static-page.service'
import { BrandingService } from './services/current-user/branding.service'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  showHeader = false
  // TODO: this shouln't have hard coded routes. its gross.
  hiddenRoutes = ['/login', '/register', '/forgot-password', '/reset-password']
  stylesheetUrl: string

  constructor(
    public context: ShopperContextService,
    private staticPages: StaticPageService,
    private branding: BrandingService
  ) {}

  ngOnInit(): void {
    this.context.router.setPageTitle();
    this.context.router.onUrlChange((url) => {
      this.showHeader = !this.hiddenRoutes.some((el) => url.includes(el))
      if(!url.includes('products/')) {
        this.context.router.setPageTitle(this.getPageTitle(url));
      }
    })
    this.staticPages.initialize()

    this.branding.Apply()
  }

  getPageTitle(url: string): string {
    //  handle case where there are query params. Don't need these in page title
    if(url.includes('?')) {
      url = url.split("?")[0];
    }
    const routeArray = url.split("/")
    const title = routeArray[routeArray.length - 1];
    return title.charAt(0).toUpperCase() + title.slice(1)
  }
}
