'use client'

import React, { useEffect, useState } from 'react';
import AuctionCard from './AuctionCard';
import { Auction } from '../../../types';
import AppPagination from '../components/AppPagination';
import { getData } from '../actions/auctionActions';
import Filters from './Filters';
import { useParamsStore } from '../hooks/useParamsStore';
import { useShallow } from 'zustand/shallow';
import qs from 'query-string';
import EmptyFilter from '../components/EmptyFilter';
import { useAuctionStore } from '../hooks/useAucitonStore';

export default function Listings() {
    const [isLoading, setIsLoading] = useState(true);
    const params = useParamsStore(useShallow(state => ({
        pageIndex: state.pageIndex,
        pageSize: state.pageSize,
        pageCount: state.pageCount,
        searchValue: state.searchValue,
        orderBy: state.orderBy,
        filterBy: state.filterBy
    })));

    const data = useAuctionStore(useShallow(state => ({
        auctions: state.auctions,
        pageIndex: state.pageIndex,
        pageSize: state.pageSize,
        pageCount: state.pageCount,
        totalPage: state.totalPage
    })));
    const setData = useAuctionStore(state => state.setData);
    // const setCurrentPrice = useAuctionStore(state => state.setCurrentPrice);

    const setParams = useParamsStore(state => state.setParams);
    const queryUrl = qs.stringifyUrl({ url: '', query: params });

    function setPageNumber(pageIndex: number) {
        setParams({ pageIndex });
    }

    useEffect(() => {
        console.log(queryUrl)
        getData(queryUrl).then(res => {
            setData(res.data);
            setIsLoading(false);
        });
    }, [queryUrl, setData])

    if (isLoading) return <h3>Loading...</h3>;

    return (
        <>
            <Filters />
            {data.auctions.length === 0 ? (
                <EmptyFilter showReset />
            ) : (
                <>
                    <div className='grid grid-cols-4 gap-6'>
                        {(data.auctions && data.auctions.length > 0 &&
                            data.auctions.map((auction: Auction) => (
                                <AuctionCard auction={auction} key={auction.id} />
                            ))
                        )}
                    </div>
                    <div className='flex justify-center mt-4'>
                        <AppPagination
                            currentPage={params.pageIndex}
                            totalPage={data.totalPage}
                            onPageChanged={setPageNumber}
                        />
                    </div>
                </>
            )}
        </>
    )
}
