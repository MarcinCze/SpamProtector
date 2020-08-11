import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { StoreModule } from '@ngrx/store';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { environment } from '../environments/environment';
import { EffectsModule } from '@ngrx/effects';
import { LoginComponent } from './components/login/login.component';
import { FilterSubjectsComponent } from './components/filter/filter-subjects/filter-subjects.component';
import { FilterSendersComponent } from './components/filter/filter-senders/filter-senders.component';
import { FilterDomainsComponent } from './components/filter/filter-domains/filter-domains.component';
import { FilterItemComponent } from './components/filter/filter-item/filter-item.component';
import { FilterAddComponent } from './components/filter/filter-add/filter-add.component';
import { FilterEditComponent } from './components/filter/filter-edit/filter-edit.component';
import { FilterListComponent } from './components/filter/filter-list/filter-list.component';
import { HomeComponent } from './components/home/home.component';
import { HeaderComponent } from './components/header/header.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    FilterSubjectsComponent,
    FilterSendersComponent,
    FilterDomainsComponent,
    FilterItemComponent,
    FilterAddComponent,
    FilterEditComponent,
    FilterListComponent,
    HomeComponent,
    HeaderComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    StoreModule.forRoot({}, {}),
    StoreDevtoolsModule.instrument({ maxAge: 25, logOnly: environment.production }),
    EffectsModule.forRoot([])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
