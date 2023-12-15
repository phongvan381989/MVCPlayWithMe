function objOrderPay(type, value) {
    if (DEBUG) {
        console.log("objOrderPay CALL ");
    }
    this.type = type;
    this.value = value;
}