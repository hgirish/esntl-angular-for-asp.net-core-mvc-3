export class OrderConfirmation {
  constructor(public orderId: number, public authCode: string, public amount: number) { }
}
