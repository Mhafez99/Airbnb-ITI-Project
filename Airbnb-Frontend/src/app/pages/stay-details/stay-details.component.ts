import {
  Component,
  OnInit,
  OnDestroy,
  ViewChild,
  ElementRef,
  Renderer2,
} from '@angular/core';
import { Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { StatReviews, Stay } from 'src/app/models/stay.model';
import {
  faArrowUpFromBracket,
  faHeart,
  faStar,
  faCircle,
} from '@fortawesome/free-solid-svg-icons';
import { UserService } from 'src/app/services/user.service';
import { User } from 'src/app/models/user.model';
import { StayService } from 'src/app/services/stay.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { OrderService } from 'src/app/services/order.service';
import { Order } from 'src/app/models/order.model';
import { ViewportScroller } from '@angular/common';

@Component({
  selector: 'stay-details',
  templateUrl: './stay-details.component.html',
  styleUrls: ['./stay-details.component.scss'],
})
export class StayDetailsComponent implements OnInit, OnDestroy {
  @ViewChild('element') element: any;

  constructor(
    private renderer2: Renderer2,
    private route: ActivatedRoute,
    private userService: UserService,
    private stayService: StayService,
    private snackBar: MatSnackBar,
    private orderService: OrderService,
    private viewportScroller: ViewportScroller
  ) {}

  unlistener!: () => void;
  user!: User;
  stay!: Stay;
  order!: Order;
  staySubscription!: Subscription;
  orderSubscription!: Subscription;
  shareIcon = faArrowUpFromBracket;
  heartIcon = faHeart;
  starIcon = faStar;
  point = faCircle;
  isShowHeader: boolean = false;
  isShowHeaderOrder: boolean = false;
  isReserveClick: boolean = false;
  isLikeByUser: boolean = false;

  ngOnInit(): void {
    this.orderSubscription = this.orderService.order$.subscribe(
      (order) => (this.order = order)
    );
    this.staySubscription = this.route.data.subscribe((data) => {
      this.stay = data['stay'];
    });
    this.user = this.userService.getUser();
    this.isLikeActive();
  }

  scrollTo(nav: string) {
    this.viewportScroller.scrollToAnchor(nav);
  }

  setIsReserveClick(val: boolean) {
    this.isReserveClick = val;
  }

  ngAfterViewInit(): void {
    this.unlistener = this.renderer2.listen('window', 'scroll', () =>
      this.onScroll(this.element)
    );
  }

  onScroll(element: ElementRef) {
    if (window.scrollY >= element.nativeElement.offsetTop)
      this.isShowHeader = true;
    else this.isShowHeader = false;
    if (window.scrollY >= 1378) this.isShowHeaderOrder = true;
    else this.isShowHeaderOrder = false;
  }

  getRateAvg() {
    let rate = 0;
    let key: keyof StatReviews;
    for (key in this.stay.statReviews) {
      rate += this.stay.statReviews[key];
    }

    return (rate / 6).toFixed(2);
  }

  isLikeActive() {
    if (!this.user) this.isLikeByUser = false;
    else
      this.isLikeByUser = this.stay.likedByUsers.some(
        (userId: number) => userId === Number(this.user.id)
      );
  }

  async onClickLike(ev: Event) {
    ev.stopPropagation();

    if (!this.user) {
      this.snackBar.open('Please login first', 'Close', { duration: 3000 });
      return;
    }

    try {
      this.stayService
        .toggleLike(Number(this.stay.id), Number(this.user.id))
        .subscribe({
          next: (updatedStay: Stay) => {
            this.stay = updatedStay;
            this.isLikeByUser = this.stay.likedByUsers.some(
              (userId: number) => userId === Number(this.user.id)
            );

            const msg = this.isLikeByUser
              ? 'Stay added to wishlist'
              : 'Stay removed from wishlist';
            this.snackBar.open(msg, 'Close', { duration: 3000 });
          },
          error: (err) => {
            console.error('Like toggle failed:', err);
            this.snackBar.open('Something went wrong', 'Close', {
              duration: 3000,
            });
          },
        });
    } catch (err) {
      console.error('err:', err);
    }
  }

  addOrder() {
    const user = this.userService.getUser();
    if (!user)
      this.snackBar.open('Please login first', 'Close', { duration: 3000 });
    else {
      this.order.host.id = this.stay.host.id;
      this.order.host.fullname = this.stay.host.fullname;
      this.order.buyer = { id: user.id, fullname: user.fullname };
      this.order.totalPrice = this.TotalPrice;
      this.order.stay = {
        id: this.stay.id,
        name: this.stay.name,
        price: this.stay.price,
      };
      this.isReserveClick = true;
    }
  }

  get TotalPrice() {
    const price =
      +this.stay.price *
      (this.order.endDate.getDate() - this.order.startDate.getDate());
    const cleanTax = +(price * 0.1).toFixed();
    const serviceFee = +(price * 0.17).toFixed();
    return price + cleanTax + serviceFee;
  }

  onClickShare() {
    navigator.clipboard.writeText(window.location.href);
    this.snackBar.open('copy to clipboard', 'Close', { duration: 3000 });
  }

  ngOnDestroy(): void {
    this.staySubscription.unsubscribe();
    this.orderSubscription.unsubscribe();
    this.unlistener();
  }
}
