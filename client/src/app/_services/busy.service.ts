import { IcuPlaceholder } from '@angular/compiler/src/i18n/i18n_ast';
import { Injectable } from '@angular/core';
import { NgxGalleryService } from '@kolkov/ngx-gallery';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  busyRequestCount = 0;

  constructor(private spinner: NgxSpinnerService) { }

  busy(){
    this.busyRequestCount++;
    this.spinner.show(undefined, {
      type: 'line-scale-party',
      bdColor: 'rgb(255,255,255,0)',
      color: '#333333'
    });
  }

  idle(){
    this.busyRequestCount--;
    if(this.busyRequestCount <= 0)
    {
      this.busyRequestCount = 0;
      this.spinner.hide();
    }
    
  }
}
