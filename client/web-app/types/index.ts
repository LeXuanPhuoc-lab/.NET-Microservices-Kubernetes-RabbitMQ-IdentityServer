export interface BaseResponse<T> {
  statusCode: number;
  message: string;
  data: T;
}

export interface PagedResult<T> {
  result: T[];
  pageIndex: number;
  totalPage: number;
}

export interface Auction {
  reservePrice: number;
  seller: string;
  winner?: string;
  soldAmount: number;
  currentHighBid: number;
  createdAt: string;
  updatedAt: string;
  auctionEnd: string;
  status: string;
  item: Item;
  id: string;
}

export interface Item {
  make: string;
  model: string;
  year: number;
  color: string;
  mileage: number;
  imageUrl: string;
  id: string;
}

export interface Bid {
  id: string;
  auctionId: string;
  bidder: string;
  bidTime: string;
  amount: number;
  bidStatus: string;
}
