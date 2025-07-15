"use server";

import { Auction, BaseResponse, PagedResult } from "../../../types";

export async function getData(
  query: string
): Promise<BaseResponse<PagedResult<Auction>>> {
  const res = await fetch(`${process.env.API_URL}/search${query}`);

  if (res.status === 404)
    return {
      statusCode: 404,
      message: "Not found",
      data: { result: [], pageIndex: 0, totalPage: 0 },
    };

  if (!res.ok) throw new Error("Failed to fetch data");

  return res.json();
}
