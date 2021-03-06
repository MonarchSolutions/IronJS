// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/**
* @name: S15.10.6.4_A9;
* @section: 15.10.6.4;
* @assertion: The RegExp.prototype.toString.length property has the attribute DontDelete;
* @description: Checking if deleting the RegExp.prototype.toString.length property fails;
*/

//CHECK#0
if ((RegExp.prototype.toString.hasOwnProperty('length') !== true)) {
	$FAIL('#0: RegExp.prototype.toString.hasOwnProperty(\'length\') === true');
}

//CHECK#1
if (delete RegExp.prototype.toString.length !== false) {
	$ERROR('#1: delete RegExp.prototype.toString.length === false');
}

//CHECK#2
if (RegExp.prototype.toString.hasOwnProperty('length') !== true) {
	$ERROR('#2: delete RegExp.prototype.toString.length; RegExp.prototype.toString.hasOwnProperty(\'length\') === true');
}

