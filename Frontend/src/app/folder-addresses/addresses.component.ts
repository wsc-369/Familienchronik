import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { Address, Person } from '../api/api-interfaces';
import { AddressService } from '../folder-services/address.service';

@Component({
  selector: 'app-addresses',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './addresses.component.html',
  styleUrls: ['./addresses.component.css']
})
export class AddressesComponent implements OnInit {
  addresses: Address[] = [];
  selectedAddress: Address = this.createEmptyAddress();
  isLoading = false;
  isSaving = false;
  errorMessage = '';
  successMessage = '';

  constructor(private readonly addressService: AddressService) {}

  ngOnInit(): void {
    this.loadAddresses();
  }

  trackByAddressId(index: number, item: Address): string {
    return item.id || `address-${index}`;
  }

  onCreateNew(): void {
    this.successMessage = '';
    this.errorMessage = '';
    this.selectedAddress = this.createEmptyAddress();
  }

  onSelectAddress(address: Address): void {
    this.successMessage = '';
    this.errorMessage = '';
    this.selectedAddress = this.cloneAddress(address);
  }

  onSaveAddress(): void {
    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const payload = this.normalizeAddress(this.selectedAddress);
    const request$ = payload.id
      ? this.addressService.updateAddress(payload.id, payload)
      : this.addressService.createAddress(payload);

    request$
      .pipe(finalize(() => {
        this.isSaving = false;
      }))
      .subscribe({
        next: (savedAddress) => {
          this.successMessage = 'Adresse wurde gespeichert.';
          this.selectedAddress = this.cloneAddress(savedAddress);
          this.loadAddresses(savedAddress.id);
        },
        error: () => {
          this.errorMessage = 'Adresse konnte nicht gespeichert werden.';
        }
      });
  }

  onDeleteAddress(): void {
    if (!this.selectedAddress.id) {
      this.onCreateNew();
      return;
    }

    const confirmed = window.confirm('Soll diese Adresse wirklich geloescht werden?');
    if (!confirmed) {
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.addressService
      .deleteAddress(this.selectedAddress.id)
      .pipe(finalize(() => {
        this.isSaving = false;
      }))
      .subscribe({
        next: () => {
          this.successMessage = 'Adresse wurde geloescht.';
          this.selectedAddress = this.createEmptyAddress();
          this.loadAddresses();
        },
        error: () => {
          this.errorMessage = 'Adresse konnte nicht geloescht werden.';
        }
      });
  }

  private loadAddresses(selectId?: string): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.addressService
      .getAddresses()
      .pipe(finalize(() => {
        this.isLoading = false;
      }))
      .subscribe({
        next: (addresses) => {
          this.addresses = addresses ?? [];

          if (selectId) {
            const selected = this.addresses.find((item) => item.id === selectId);
            if (selected) {
              this.selectedAddress = this.cloneAddress(selected);
            }
          }
        },
        error: () => {
          this.errorMessage = 'Adressen konnten nicht geladen werden.';
        }
      });
  }

  private createEmptyAddress(): Address {
    return {
      id: '',
      person: this.createEmptyPerson(),
      personRefId: '',
      street: '',
      houseNr: '',
      town: '',
      zip: '',
      country: '',
      fullAddress: '',
      orderNr: 0,
      description: '',
      active: true
    };
  }

  private createEmptyPerson(): Person {
    return {
      Id: '',
      personId: '',
      firstName: '',
      familyName: '',
      genderStatus: 0,
      genderStatusName: '',
      active: true
    };
  }

  private cloneAddress(address: Address): Address {
    return {
      ...address,
      person: {
        ...this.createEmptyPerson(),
        ...(address.person ?? this.createEmptyPerson())
      }
    };
  }

  private normalizeAddress(address: Address): Address {
    const person = {
      ...this.createEmptyPerson(),
      ...(address.person ?? this.createEmptyPerson())
    };

    return {
      ...address,
      id: address.id?.trim(),
      personRefId: address.personRefId?.trim(),
      street: address.street?.trim(),
      houseNr: address.houseNr?.trim(),
      town: address.town?.trim(),
      zip: address.zip?.trim(),
      country: address.country?.trim(),
      fullAddress: address.fullAddress?.trim(),
      description: address.description?.trim(),
      person: {
        ...person,
        Id: person.Id?.trim(),
        personId: person.personId?.trim(),
        firstName: person.firstName?.trim(),
        familyName: person.familyName?.trim(),
        genderStatusName: person.genderStatusName?.trim()
      }
    };
  }
}
