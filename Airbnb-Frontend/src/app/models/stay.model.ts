export interface Stay {
  id: string;
  type: string;
  name: string;
  imgUrls: string[];
  price: number;
  summary: string;
  capacity: number;
  amenities: string[];
  labels: string[];
  host: Host;
  loc: Loc;
  reviews: Review[];
  likedByUsers: number[];
  roomType: string;
  statReviews: StatReviews;
  bathrooms: number;
  bedrooms: number;
}
export interface Host {
  id: string;
  fullname: string;
  pictureUrl: string;
  createAt: number;
  isSuperhost: boolean;
  policyNumber: number;
  responseTime: string;
}

export interface Loc {
  country: string;
  countryCode: string;
  city: string;
  address: string;
  lat: number;
  lan: number;
}
export interface googleMapLoc {
  lat: number;
  lng: number;
}

export interface Review {
  at: number;
  txt: string;
  by: {
    id: string;
    fullname: string;
    imgUrl: string;
  };
  statReviews: StatReviews;
}

export interface StatReviews {
  cleanliness: number;
  communication: number;
  checkIn: number;
  accuracy: number;
  location: number;
  value: number;
}

export interface StayFilter {
  likeByUser: string;
  place: string;
  label: string;
  hostId: string;
  isPetAllowed: string;
}
