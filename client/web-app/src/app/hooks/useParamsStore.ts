import { create } from "zustand";

type State = {
  pageIndex: number;
  pageSize: number;
  pageCount: number;
  searchValue: string;
  orderBy: string;
  filterBy: string;
};

type Actions = {
  setParams: (params: Partial<State>) => void;
  resetParams: () => void;
  setSearchValue: (value: string) => void;
};

const initialState: State = {
  pageIndex: 1,
  pageSize: 12,
  pageCount: 1,
  searchValue: "",
  orderBy: "",
  filterBy: "",
};

export const useParamsStore = create<State & Actions>((set) => ({
  ...initialState,
  setParams: (newParams: Partial<State>) => {
    set((state) => {
      if (newParams.pageIndex) {
        return { ...state, pageIndex: newParams.pageIndex };
      } else {
        return { ...state, ...newParams, pageIndex: 1 };
      }
    });
  },
  resetParams: () => set(initialState),
  setSearchValue: (value: string) => {
    set({ searchValue: value });
  },
}));
