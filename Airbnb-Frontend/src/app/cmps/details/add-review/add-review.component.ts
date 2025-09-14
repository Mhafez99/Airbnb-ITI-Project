import { Component, Input } from '@angular/core';
import { Review, StatReviews, Stay } from 'src/app/models/stay.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UserService } from 'src/app/services/user.service';
import { StayService } from 'src/app/services/stay.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'add-review',
  templateUrl: './add-review.component.html',
  styleUrls: ['./add-review.component.scss'],
})
export class AddReviewComponent {
  @Input() stay!: Stay;
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private stayService: StayService,
    private snackBar: MatSnackBar
  ) {
    this.form = this.fb.group({
      cleanliness: [5, Validators.required],
      communication: [5, Validators.required],
      checkIn: [5, Validators.required],
      accuracy: [5, Validators.required],
      location: [5, Validators.required],
      value: [5, Validators.required],
      text: ['', Validators.required],
    });
  }
  ngOnChanges() {
    if (this.stay && this.stay.statReviews) {
      this.form.patchValue({
        cleanliness: this.stay.statReviews.cleanliness ?? 5,
        communication: this.stay.statReviews.communication ?? 5,
        checkIn: this.stay.statReviews.checkIn ?? 5,
        accuracy: this.stay.statReviews.accuracy ?? 5,
        location: this.stay.statReviews.location ?? 5,
        value: this.stay.statReviews.value ?? 5,
      });
    }
  }

  makeReview(user: any): Review {
    const review: Review = this.stayService.getEmptyReview();
    // const statReviews: StatReviews = this.form.value.statReviews;

    review.txt = this.form.value.text;
    review.by = user;
    review.statReviews = {
      cleanliness: this.form.value.cleanliness ?? 0,
      communication: this.form.value.communication ?? 0,
      checkIn: this.form.value.checkIn ?? 0,
      accuracy: this.form.value.accuracy ?? 0,
      location: this.form.value.location ?? 0,
      value: this.form.value.value ?? 0,
    };

    return review;
  }

  makeStarRate(statReviews: StatReviews): StatReviews {
    if (!this.stay.statReviews) {
      this.stay.statReviews = {
        cleanliness: 0,
        communication: 0,
        checkIn: 0,
        accuracy: 0,
        location: 0,
        value: 0,
      };
    }

    let key: keyof StatReviews;
    const length = this.stay.reviews.length;

    for (key in statReviews) {
      const prevValue = this.stay.statReviews[key] || 0;
      const rate = (prevValue * (length - 1) + statReviews[key]) / length;
      statReviews[key] = +rate.toFixed(2);
    }

    return statReviews;
  }

  async onAddReview() {
    const user = this.userService.getUser();
    if (!user) {
      this.snackBar.open('Please login first', 'Close', { duration: 3000 });
      return;
    }

    if (!this.form.value.text) {
      this.snackBar.open('Please add review text', 'Close', { duration: 3000 });
      return;
    }

    const review = this.makeReview(user);

    const dto: any = {
      txt: review.txt,
      reviewerId: review.by.id.toString(),
      reviewerFullname: review.by.fullname,
      reviewerImgUrl: review.by.imgUrl,
      StatReviews: {
        Cleanliness: review.statReviews.cleanliness || 5,
        Communication: review.statReviews.communication || 5,
        CheckIn: review.statReviews.checkIn || 5,
        Accuracy: review.statReviews.accuracy || 5,
        Location: review.statReviews.location || 5,
        Value: review.statReviews.value || 5,
      },
    };

    this.stay.reviews.unshift(review);

    const savedReview = await this.stayService.addReview(+this.stay.id, dto);

    if (savedReview.statReviews) {
      this.stay.statReviews = savedReview.statReviews;
    }

    this.snackBar.open('Review added successfully', 'Close', {
      duration: 3000,
    });
  }
}
