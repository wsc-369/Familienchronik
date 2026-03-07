import { Component } from '@angular/core';
import { Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Person } from '../folder-interfaces/person ';
import { ROOT_PERSON } from '../folder-models/family-data';

@Component({
    selector: 'app-family-tree',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './famliy-tree.component.html',
    styleUrls: ['./famliy-tree.component.css']
})
export class FamilyTreeComponent {
    // rootPerson = ROOT_PERSON;
    @Input() person: Person =  ROOT_PERSON;
     //rootPerson = ROOT_PERSON;
}