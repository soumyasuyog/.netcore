import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-button-groups',
  templateUrl: './button-groups.component.html',
  styleUrls: ['./button-groups.component.scss']
})
export class ButtonGroupsComponent {

  formCheck1: FormGroup;
  formRadio1: FormGroup;

  constructor(
    private formBuilder: FormBuilder
  ) {
    this.formCheck1 = this.formBuilder.group({
      checkbox1: false,
      checkbox2: false,
      checkbox3: false
    });

    this.formRadio1 = this.formBuilder.group({
      radio1: new FormControl('Radio1')
    });
  }

  setCheckBoxValue(controlName: string) {
    const prevValue = this.formCheck1.get(controlName)?.value;
    const value = this.formCheck1.value;
    value[controlName] = !prevValue;
    this.formCheck1.setValue(value);
  }

  setRadioValue(value: string): void {
    this.formRadio1.setValue({ radio1: value });
  }
}
