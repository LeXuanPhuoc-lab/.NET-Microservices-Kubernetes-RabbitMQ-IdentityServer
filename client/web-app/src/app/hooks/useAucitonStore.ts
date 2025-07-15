import { create } from "zustand";
import { Auction, PagedResult } from "../../../types";

type State = {
  auctions: Auction[];
  pageIndex: number;
  pageSize: number;
  pageCount: number;
  totalPage: number;
};

type Actions = {
  setData: (data: PagedResult<Auction>) => void;
  setCurrentPrice: (auctionId: string, amount: number) => void;
};

const initialState: State = {
  auctions: [],
  pageIndex: 0,
  pageSize: 0,
  pageCount: 0,
  totalPage: 0,
};

export const useAuctionStore = create<State & Actions>((set) => ({
  ...initialState,

  setData: (data: PagedResult<Auction>) => {
    set({
      auctions: data.result,
      pageIndex: data.pageIndex,
      pageSize: data.result.length,
      pageCount: data.totalPage,
    });
  },

  setCurrentPrice: (auctionId: string, amount: number) => {
    set((state) => ({
      auctions: state.auctions.map((auction) =>
        auction.id === auctionId
          ? { ...auction, currentHighBid: amount }
          : auction
      ),
    }));
  },
}));
