// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/**
 * @name: S15.9.5.32_A3_T3;
 * @section: 15.9.5.32;
 * @assertion: The Date.prototype.setMinutes property "length" has { ReadOnly, DontDelete, DontEnum } attributes;
 * @description: Checking DontEnum attribute;
 */

if (Date.prototype.setMinutes.propertyIsEnumerable('length')) {
  $ERROR('#1: The Date.prototype.setMinutes.length property has the attribute DontEnum');
}

for(x in Date.prototype.setMinutes) {
  if(x === "length") {
    $ERROR('#2: The Date.prototype.setMinutes.length has the attribute DontEnum');
  }
}

